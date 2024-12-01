using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace shared.Messaging.RabbitMQ
{
    public class RabbitMQSubscriber : IRabbitMQSubscriber
    {
        private readonly IConnection _connection;

        public RabbitMQSubscriber(IConnection connection)
        {
            _connection = connection;
        }

        public void Subscribe<T>(string queueName, Func<T, Task> onMessageReceived) where T : class
        {
            var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var messageString = Encoding.UTF8.GetString(body);
                var message = JsonConvert.DeserializeObject<T>(messageString);
                if (message != null)
                {
                    await onMessageReceived(message);
                }

                channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }
    }
}
