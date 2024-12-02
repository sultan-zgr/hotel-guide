namespace ReportService.DTOs
{
    public class ReportDTO
    {
        public Guid Id { get; set; }
        public string Location { get; set; }
        public int HotelCount { get; set; }
        public int ContactCount { get; set; }
        public string Status { get; set; } 
        public DateTime RequestedAt { get; set; }
    }

}
