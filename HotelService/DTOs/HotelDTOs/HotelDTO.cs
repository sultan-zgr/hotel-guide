using HotelService.DTOs.ContactDTOs;

namespace HotelService.DTOs.HotelDTOs
{
    public class HotelDTO
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public List<ContactDTO> Contacts { get; set; }
    }

}
