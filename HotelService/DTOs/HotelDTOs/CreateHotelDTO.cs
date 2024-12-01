using HotelService.DTOs.ContactDTOs;

namespace HotelService.DTOs.HotelDTOs
{
    public class CreateHotelDTO
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public List<CreateContactDTO> Contacts { get; set; }
    }

}
