using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using ReportService.Data;
using ReportService.Mappings;
using ReportService.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritaban� Ba�lant�s�
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. RabbitMQ Ba�lant�s�
builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory
    {
        Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQConnection")),
        DispatchConsumersAsync = true // Asenkron t�keticiler i�in gerekli
    };
    return factory.CreateConnection();
});

// 3. AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// 4. Servisler
builder.Services.AddScoped<ReportManagementService>();
builder.Services.AddScoped<ReportProcessingService>();

// 5. Controller ve Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 6. Middleware ve Swagger Ayarlar�
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

// RabbitMQ kuyru�unu dinlemeye ba�la
using (var scope = app.Services.CreateScope())
{
    var reportProcessingService = scope.ServiceProvider.GetRequiredService<ReportProcessingService>();
    reportProcessingService.StartListening(); // Kuyru�u dinlemeye ba�lar
}

app.Run();
