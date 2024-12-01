using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using ReportService.Data;
using ReportService.Mappings;
using ReportService.Services;
using shared.Messaging.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

// Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// RabbitMQ Connection
builder.Services.AddSingleton<IConnection>(sp =>
{
    var rabbitMqConnection = builder.Configuration["RabbitMQ:Connection"];
    if (string.IsNullOrEmpty(rabbitMqConnection))
    {
        throw new ArgumentNullException("RabbitMQ:Connection", "RabbitMQ connection string is not configured properly.");
    }

    var factory = new ConnectionFactory
    {
        Uri = new Uri(rabbitMqConnection),
        DispatchConsumersAsync = true // Asynchronous consumers
    };

    return factory.CreateConnection();
});

// Add RabbitMQSubscriber
builder.Services.AddSingleton<IRabbitMQSubscriber, RabbitMQSubscriber>();

// Hosted Worker Service
builder.Services.AddHostedService<Worker>();

// AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Dependency Injection for Services
builder.Services.AddScoped<ReportManagementService>();
builder.Services.AddSingleton<HotelEventListener>();

// Add Swagger and Controllers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Start HotelEventListener (RabbitMQ Listener for Hotel Events)
using (var scope = app.Services.CreateScope())
{
    var hotelEventListener = scope.ServiceProvider.GetRequiredService<HotelEventListener>();
    hotelEventListener.StartListening();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
