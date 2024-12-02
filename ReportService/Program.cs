using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using ReportService.Data;
using ReportService.Mappings;
using ReportService.Services;
using ReportService.Validations;
using Serilog;
using shared.Messaging.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() 
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) 
    .CreateLogger();

builder.Host.UseSerilog(); 

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IConnection>(sp =>
{
    var rabbitMqConnection = builder.Configuration["RabbitMQ:Connection"];
    if (string.IsNullOrEmpty(rabbitMqConnection))
    {
        Log.Error("RabbitMQ connection string is not configured properly.");
        throw new ArgumentNullException("RabbitMQ:Connection", "RabbitMQ connection string is not configured properly.");
    }

    var factory = new ConnectionFactory
    {
        Uri = new Uri(rabbitMqConnection),
        DispatchConsumersAsync = true 
    };

    try
    {
        var connection = factory.CreateConnection();
        Log.Information("Successfully connected to RabbitMQ at {RabbitMQConnection}", rabbitMqConnection);
        return connection;
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Failed to connect to RabbitMQ at {RabbitMQConnection}", rabbitMqConnection);
        throw new InvalidOperationException("RabbitMQ connection failed. Check RabbitMQ server and configuration.", ex);
    }
});

builder.Services.AddSingleton<IRabbitMQSubscriber, RabbitMQSubscriber>();

builder.Services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();

builder.Services.AddHostedService<Worker>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<CreateReportRequestDTOValidator>();
    });

builder.Services.AddScoped<ReportManagementService>();
builder.Services.AddSingleton<HotelEventListener>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var hotelEventListener = scope.ServiceProvider.GetRequiredService<HotelEventListener>();
    try
    {
        hotelEventListener.StartListening();
        Log.Information("HotelEventListener started successfully.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "HotelEventListener failed to start.");
        throw;
    }
}

app.UseHttpsRedirection();
app.UseRouting();

app.Use(async (context, next) =>
{
    Log.Information("Handling request: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
    Log.Information("Finished handling request. Status Code: {StatusCode}", context.Response.StatusCode);
});

app.MapControllers();
app.Run();
