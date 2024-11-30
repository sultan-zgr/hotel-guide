public enum HotelEventType
{
    Added,
    Updated,
    Deleted
}

public abstract class HotelEvent
{
    public HotelEventType EventType { get; set; }
}

public class HotelAddedEvent : HotelEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }

    public HotelAddedEvent()
    {
        EventType = HotelEventType.Added;
    }
}

public class HotelUpdatedEvent : HotelEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }

    public HotelUpdatedEvent()
    {
        EventType = HotelEventType.Updated;
    }
}

public class HotelDeletedEvent : HotelEvent
{
    public Guid Id { get; set; }

    public HotelDeletedEvent()
    {
        EventType = HotelEventType.Deleted;
    }
}