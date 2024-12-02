using FluentValidation.AspNetCore;
using HotelService.Data;
using HotelService.Mappings;
using HotelService.Repositories;
using HotelService.Services;
using HotelService.Validations;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Serilog;
using shared.Messaging.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Console loglama
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // Günlük dosya loglama
    .CreateLogger();

builder.Host.UseSerilog(); // Serilog'u entegre et

// Add services to the container
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
ConfigureMiddleware(app);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Swagger/OpenAPI
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    // Database Context
    services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

    // FluentValidation Integration
    services.AddControllers()
        .AddFluentValidation(fv =>
        {
            fv.RegisterValidatorsFromAssemblyContaining<CreateContactDTOValidator>();
            fv.RegisterValidatorsFromAssemblyContaining<CreateHotelDTOValidator>();
        });

    // AutoMapper
    services.AddAutoMapper(typeof(AutoMapperProfiles));

    // Dependency Injection for Repositories and Services
    services.AddScoped<IHotelRepository, HotelRepository>();
    services.AddScoped<HotelManagementService>();
    services.AddScoped<ContactService>();

    // RabbitMQ Configuration
    services.AddSingleton<IConnection>(sp =>
    {
        try
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(configuration.GetConnectionString("RabbitMQConnection")),
                DispatchConsumersAsync = true
            };
            return factory.CreateConnection();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "RabbitMQ connection error");
            throw;
        }
    });

    // RabbitMQ Publisher
    services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();

    // Controllers
    services.AddControllers();
}

void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    // Routing Middleware
    app.UseRouting();

    // Log All Requests/Responses (Middleware Example)
    app.Use(async (context, next) =>
    {
        Log.Information("Handling request: {Method} {Path}", context.Request.Method, context.Request.Path);
        await next();
        Log.Information("Finished handling request. Status Code: {StatusCode}", context.Response.StatusCode);
    });

    // Controller Endpoint Mapping
    app.MapControllers();
}
