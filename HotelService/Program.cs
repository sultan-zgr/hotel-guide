using FluentValidation.AspNetCore;
using HotelService.Data;
using HotelService.Mappings;
using HotelService.Services;
using HotelService.Validations;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using shared.Messaging.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// FluentValidation Integration
builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<CreateContactDTOValidator>(); // Contact Validasyonlarý
        fv.RegisterValidatorsFromAssemblyContaining<CreateHotelDTOValidator>();   // Hotel Validasyonlarý
    });

// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Dependency Injection for Services
builder.Services.AddScoped<HotelManagementService>();
builder.Services.AddScoped<ContactService>();

// RabbitMQ Configuration
builder.Services.AddSingleton<IConnection>(sp =>
{
    try
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQConnection")),
            DispatchConsumersAsync = true
        };
        return factory.CreateConnection();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"RabbitMQ connection error: {ex.Message}");
        throw;
    }
});

// RabbitMQ Publisher
builder.Services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();

// Controllers (Enable Controller Support)
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable Routing
app.UseRouting();

// Map Controllers
app.MapControllers();

app.Run();
