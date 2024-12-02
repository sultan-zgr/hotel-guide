namespace ReportService.Models
{
    public class Report
    {
        public Guid Id { get; set; } = Guid.NewGuid(); 
        public string Location { get; set; } 
        public int HotelCount { get; set; } = 0; 
        public int ContactCount { get; set; } = 0; 
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow; 
        public string Status { get; set; } = "Preparing"; 
    }
}
