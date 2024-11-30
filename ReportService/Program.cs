using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using ReportService.Data;
using ReportService.Mappings;
using ReportService.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabaný Baðlantýsý
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. RabbitMQ Baðlantýsý
builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory
    {
        Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQConnection")),
        DispatchConsumersAsync = true // Asenkron tüketiciler için gerekli
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

// 6. Middleware ve Swagger Ayarlarý
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

// RabbitMQ kuyruðunu dinlemeye baþla
using (var scope = app.Services.CreateScope())
{
    var reportProcessingService = scope.ServiceProvider.GetRequiredService<ReportProcessingService>();
    reportProcessingService.StartListening(); // Kuyruðu dinlemeye baþlar
}

app.Run();
