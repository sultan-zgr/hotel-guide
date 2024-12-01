namespace ReportService.Models
{
    public class Report
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Varsayılan ID
        public string Location { get; set; } // Konum
        public int HotelCount { get; set; } = 0; // Başlangıçta 0
        public int ContactCount { get; set; } = 0; // Başlangıçta 0
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow; // Talep tarihi
        public string Status { get; set; } = "Preparing"; // Varsayılan durum
    }
}
