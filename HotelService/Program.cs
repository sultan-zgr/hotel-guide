using HotelService.Data;
using HotelService.Mappings;
using HotelService.Services;
using Microsoft.EntityFrameworkCore;
using shared.Messaging;
using Shared.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Dependency Injection for Services
builder.Services.AddScoped<HotelManagementService>();
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<ReportManagementService>();

// RabbitMQ Configuration
builder.Services.AddSingleton<IMessageQueue>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("RabbitMQConnection");
    return new RabbitMQPublisher(connectionString);
});

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
