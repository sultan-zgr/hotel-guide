namespace HotelService.Models
{
    public class ReportRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Location { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Preparing"; // Example: Preparing, Completed
    }

}
