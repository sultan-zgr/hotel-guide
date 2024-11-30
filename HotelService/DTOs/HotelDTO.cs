namespace HotelService.DTOs
{
    public class HotelDTO
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public List<ContactDTO> Contacts { get; set; }
    }

}
