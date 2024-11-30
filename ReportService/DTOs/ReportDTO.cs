namespace ReportService.DTOs
{
    public class ReportDTO
    {
        public Guid Id { get; set; }
        public string Location { get; set; }
        public int HotelCount { get; set; }
        public int PhoneCount { get; set; }
        public string Status { get; set; } // Example: Preparing, Completed
        public DateTime RequestedAt { get; set; }
    }

}
