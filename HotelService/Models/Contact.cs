namespace HotelService.Models
{
    public class Contact
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Type { get; set; } // Phone, Email, etc.
        public string Value { get; set; }
        public Guid HotelId { get; set; }
        public Hotel Hotel { get; set; }
    }

}
