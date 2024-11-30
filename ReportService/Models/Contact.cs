namespace ReportService.Models
{
    public class Contact
    {
        public Guid Id { get; set; }
        public string Type { get; set; } // Phone, Email, etc.
        public string Value { get; set; } // +90 123 456 7890, info@example.com, etc.
        public Guid HotelId { get; set; } // Foreign Key
        public Hotel Hotel { get; set; } // Navigation Property
    }
}
