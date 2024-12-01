using HotelService.DTOs.ContactDTOs;
using HotelService.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly ContactService _contactService;

        public ContactController(ContactService contactService)
        {
            _contactService = contactService;
        }

        // 1. İletişim Bilgisi Ekle
        [HttpPost("{hotelId:guid}/contacts")]
        public async Task<IActionResult> AddContact(Guid hotelId, [FromBody] CreateContactDTO contactDTO)
        {
            await _contactService.AddContact(hotelId, contactDTO);
            return StatusCode(201); // Created
        }

        // 2. İletişim Bilgisi Sil
        [HttpDelete("contacts/{id:guid}")]
        public async Task<IActionResult> DeleteContact(Guid id)
        {
            await _contactService.DeleteContact(id);
            return NoContent(); // Success but no response body
        }
    }
}
