using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using shared.Messaging;
using System.Text;

namespace Shared.Messaging
{
    public class RabbitMQPublisher : IMessageQueue
    {
        private readonly IConnection _connection;

        public RabbitMQPublisher(string connectionString)
        {
            var factory = new ConnectionFactory { Uri = new Uri(connectionString) };
            _connection = factory.CreateConnection();
        }

        // Mesajı RabbitMQ kuyruğuna gönder
        public void Publish<T>(string queueName, T message) where T : class
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        }

        // RabbitMQ kuyruğunu dinle
        public void Subscribe<T>(string queueName, Action<T> onMessageReceived) where T : class
        {
            var channel = _connection.CreateModel(); // Kanalı sürekli açık tut
            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray(); // Mesajı byte[] olarak al
                var messageString = Encoding.UTF8.GetString(body); // Byte[] -> String
                var message = JsonConvert.DeserializeObject<T>(messageString); // String -> T
                if (message != null)
                {
                    onMessageReceived(message); // Callback ile mesajı işleyin
                }

                // Mesaj başarıyla işlenince acknowledgment gönder
                channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }
    }
}
