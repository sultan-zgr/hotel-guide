using shared.Messaging.Events;

namespace shared.Messaging.RabbitMQ
{
    public interface IRabbitMQPublisher
    {
        void Publish<T>(string queueName, T message) where T : class;
        void PublishHotelAddedEvent(HotelAddedEvent hotelEvent);
        void PublishHotelUpdatedEvent(HotelUpdatedEvent hotelEvent);
        void PublishHotelDeletedEvent(HotelDeletedEvent hotelEvent);
    }
}
