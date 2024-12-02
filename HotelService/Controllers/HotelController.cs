using HotelService.DTOs.HotelDTOs;
using HotelService.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly HotelManagementService _hotelService;

        public HotelController(HotelManagementService hotelService)
        {
            _hotelService = hotelService;
        }

        // 1. Otelleri Listele
        [HttpGet]
        public async Task<IActionResult> GetAllHotels()
        {
            var hotels = await _hotelService.GetAllHotels();
            return Ok(hotels);
        }

        // 2. Yeni Otel Ekle
        [HttpPost]
        public async Task<IActionResult> AddHotel([FromBody] CreateHotelDTO hotelDTO)
        {
            await _hotelService.AddHotel(hotelDTO);
            return StatusCode(201); // Created
        }

        // 3. Otel Güncelle
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateHotel(Guid id, [FromBody] UpdateHotelDTO hotelDTO)
        {
            await _hotelService.UpdateHotel(id, hotelDTO);
            return NoContent(); // Success but no response body
        }

        // 4. Otel Sil
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteHotel(Guid id)
        {
            await _hotelService.DeleteHotel(id);
            return NoContent(); // Success but no response bodyy
        }
    }
}
