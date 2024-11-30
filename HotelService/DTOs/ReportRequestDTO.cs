namespace HotelService.DTOs
{
    public class ReportRequestDTO
    {
        public Guid Id { get; set; }
        public string Location { get; set; }
        public DateTime RequestedAt { get; set; }
        public string Status { get; set; } // Example: Preparing, Completed
    }
}
