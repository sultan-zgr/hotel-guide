using Newtonsoft.Json;
using RabbitMQ.Client;
using shared.Messaging.Events;
using System.Text;

namespace shared.Messaging.RabbitMQ
{
    public class RabbitMQPublisher : IRabbitMQPublisher
    {
        private readonly IConnection _connection;

        public RabbitMQPublisher(IConnection connection)
        {
            _connection = connection;
        }

        public void Publish<T>(string queueName, T message) where T : class
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        }

        public void PublishHotelAddedEvent(HotelAddedEvent hotelEvent)
        {
            Publish("hotel-events", hotelEvent);
        }

        public void PublishHotelUpdatedEvent(HotelUpdatedEvent hotelEvent)
        {
            Publish("hotel-events", hotelEvent);
        }

        public void PublishHotelDeletedEvent(HotelDeletedEvent hotelEvent)
        {
            Publish("hotel-events", hotelEvent);
        }
        //public void PublishReportRequest(ReportRequestEvent reportEvent)
        //{
        //    Publish("report-queue", reportEvent);
        //}

    }
}
