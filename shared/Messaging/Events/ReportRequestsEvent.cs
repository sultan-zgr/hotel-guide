namespace shared.Messaging.Events
{
    public class ReportRequestEvent
    {
        public Guid ReportId { get; set; }
        public string Location { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
