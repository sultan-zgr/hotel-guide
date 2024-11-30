namespace ReportService.Models
{
    public class Report
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Location { get; set; }
        public int HotelCount { get; set; }
        public int PhoneCount { get; set; }
        public DateTime RequestedAt { get; set; }
        public string Status { get; set; } = "Preparing"; // Example: Preparing, Completed
    }

}
