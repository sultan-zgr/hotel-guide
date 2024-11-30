namespace HotelService.Models
{
    public class Hotel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Location { get; set; }
        public List<Contact> Contacts { get; set; }
    }

}
