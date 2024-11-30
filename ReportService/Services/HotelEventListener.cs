using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using ReportService.Data;
using Newtonsoft.Json;
using ReportService.Models;
using System.Text;

namespace ReportService.Services
{
    public class HotelEventListener
    {
        private readonly IConnection _connection;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        public HotelEventListener(IConnection connection, IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _connection = connection;
            _configuration = configuration;
            _scopeFactory = scopeFactory;
        }

        public void StartListening()
        {
            var queueName = _configuration["RabbitMQ:HotelQueue"];
            var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, eventArgs) =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    try
                    {
                        var body = eventArgs.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var hotelEvent = JsonConvert.DeserializeObject<HotelUpdatedEvent>(message);

                        if (hotelEvent != null)
                        {
                            var existingHotel = await dbContext.Hotels.FindAsync(hotelEvent.Id);
                            if (existingHotel == null)
                            {
                                var newHotel = new Hotel
                                {
                                    Id = hotelEvent.Id,
                                    Name = hotelEvent.Name,
                                    Location = hotelEvent.Location
                                };
                                dbContext.Hotels.Add(newHotel);
                            }
                            else
                            {
                                existingHotel.Name = hotelEvent.Name;
                                existingHotel.Location = hotelEvent.Location;
                                dbContext.Hotels.Update(existingHotel);
                            }

                            await dbContext.SaveChangesAsync();
                        }

                        channel.BasicAck(eventArgs.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing hotel event: {ex.Message}");
                        channel.BasicNack(eventArgs.DeliveryTag, false, requeue: true);
                    }
                }
            };

            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }
    }
}
