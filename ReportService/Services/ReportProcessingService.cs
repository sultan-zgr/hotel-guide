using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using ReportService.DTOs;
using ReportService.Models;
using System.Text;
using AutoMapper;
using ReportService.Data;

namespace ReportService.Services
{
    public class ReportProcessingService
    {
        private readonly AppDbContext _context;
        private readonly IConnection _connection;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ReportProcessingService(AppDbContext context, IConnection connection, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _connection = connection;
            _mapper = mapper;
            _configuration = configuration;
        }

        public void StartListening()
        {
            var queueName = _configuration["RabbitMQ:QueueName"];
            var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            // Asenkron tüketici kullanıyoruz
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, eventArgs) =>
            {
                try
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var reportRequestDTO = JsonConvert.DeserializeObject<CreateReportRequestDTO>(message);

                    if (reportRequestDTO != null)
                    {
                        var report = _mapper.Map<Report>(reportRequestDTO);
                        report.Status = "Completed";
                        report.HotelCount = GetHotelCount(report.Location);

                        _context.Reports.Add(report);
                        await _context.SaveChangesAsync();
                    }

                    // Mesaj başarıyla işlendi, RabbitMQ'ya acknowledge gönder
                    channel.BasicAck(eventArgs.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Message processing failed: {ex.Message}");
                    // Mesaj işlenemezse RabbitMQ'ya reject gönder
                    channel.BasicNack(eventArgs.DeliveryTag, false, requeue: true);
                }
            };

            // Kuyruğu tüketmek için asenkron consumer kullanımı
            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }

        private int GetHotelCount(string location)
        {
            // Veritabanında belirtilen konumdaki otelleri say
            return _context.Hotels.Count(h => h.Location == location);
        }
    }
}
