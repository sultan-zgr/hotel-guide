using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using ReportService.Data;
using ReportService.Mappings;
using ReportService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

// Database Context
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// RabbitMQ Connection
builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory
    {
        Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQConnection")),
        DispatchConsumersAsync = true
    };
    return factory.CreateConnection();
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Dependency Injection for Services
builder.Services.AddScoped<ReportManagementService>();
builder.Services.AddScoped<ReportProcessingService>();
builder.Services.AddSingleton<HotelEventListener>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Start Listening to RabbitMQ
using (var scope = app.Services.CreateScope())
{
    var processingService = scope.ServiceProvider.GetRequiredService<ReportProcessingService>();
    var hotelEventListener = scope.ServiceProvider.GetRequiredService<HotelEventListener>();

    // Start Listening for Messages
    processingService.StartListening();
    hotelEventListener.StartListening();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
