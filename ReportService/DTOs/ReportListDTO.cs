namespace ReportService.DTOs
{
    public class ReportListDTO
    {
        public Guid Id { get; set; }
        public string Location { get; set; }
        public int HotelCount { get; set; }
        public DateTime RequestedAt { get; set; }
        public string Status { get; set; }
    }
}
