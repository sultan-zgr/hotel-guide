namespace shared.Messaging.RabbitMQ
{
    public interface IRabbitMQSubscriber
    {
        void Subscribe<T>(string queueName, Func<T, Task> onMessageReceived) where T : class;
    }
}
