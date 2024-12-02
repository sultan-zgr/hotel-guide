using HotelService.DTOs.HotelDTOs;
using HotelService.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using FluentValidation;
using FluentValidation.Results;

namespace HotelService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly HotelManagementService _hotelService;
        private readonly IValidator<CreateHotelDTO> _createHotelValidator;
        private readonly IValidator<UpdateHotelDTO> _updateHotelValidator;

        public HotelController(
            HotelManagementService hotelService,
            IValidator<CreateHotelDTO> createHotelValidator,
            IValidator<UpdateHotelDTO> updateHotelValidator)
        {
            _hotelService = hotelService;
            _createHotelValidator = createHotelValidator;
            _updateHotelValidator = updateHotelValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHotels()
        {
            Log.Information("Fetching all hotels.");
            var hotels = await _hotelService.GetAllHotels();
            return Ok(new { Message = "Hotels retrieved successfully.", Data = hotels });
        }

        [HttpPost]
        public async Task<IActionResult> AddHotel([FromBody] CreateHotelDTO hotelDTO)
        {
            ValidationResult validationResult = await _createHotelValidator.ValidateAsync(hotelDTO);

            if (!validationResult.IsValid)
            {
                Log.Warning("Validation failed for CreateHotelDTO: {Errors}", validationResult.Errors);
                return BadRequest(new
                {
                    Message = "Validation failed for the provided hotel data.",
                    Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }

            try
            {
                await _hotelService.AddHotel(hotelDTO);
                Log.Information("Hotel added successfully: {HotelName}", hotelDTO.Name);
                return StatusCode(201, new { Message = "Hotel added successfully." });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error adding hotel: {HotelName}", hotelDTO.Name);
                return StatusCode(500, new { Message = "An error occurred while adding the hotel." });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateHotel(Guid id, [FromBody] UpdateHotelDTO hotelDTO)
        {
            ValidationResult validationResult = await _updateHotelValidator.ValidateAsync(hotelDTO);

            if (!validationResult.IsValid)
            {
                Log.Warning("Validation failed for UpdateHotelDTO: {Errors}", validationResult.Errors);
                return BadRequest(new
                {
                    Message = "Validation failed for the provided hotel data.",
                    Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }

            try
            {
                await _hotelService.UpdateHotel(id, hotelDTO);
                Log.Information("Hotel updated successfully: {HotelId}", id);
                return Ok(new { Message = "Hotel updated successfully." });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating hotel: {HotelId}", id);
                return StatusCode(500, new { Message = "An error occurred while updating the hotel." });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteHotel(Guid id)
        {
            try
            {
                await _hotelService.DeleteHotel(id);
                Log.Information("Hotel deleted successfully: {HotelId}", id);
                return Ok(new { Message = "Hotel deleted successfully." });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting hotel: {HotelId}", id);
                return StatusCode(500, new { Message = "An error occurred while deleting the hotel." });
            }
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> AddHotelsBulk([FromBody] List<CreateHotelDTO> hotelsDTO)
        {
            foreach (var hotelDTO in hotelsDTO)
            {
                ValidationResult validationResult = await _createHotelValidator.ValidateAsync(hotelDTO);
                if (!validationResult.IsValid)
                {
                    Log.Warning("Validation failed for one of the hotels in bulk: {Errors}", validationResult.Errors);
                    return BadRequest(new
                    {
                        Message = "Validation failed for one or more hotels.",
                        Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                    });
                }
            }

            try
            {
                await _hotelService.AddHotelsBulk(hotelsDTO);
                Log.Information("Bulk hotels added successfully.");
                return StatusCode(201, new { Message = "Hotels added in bulk successfully." });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error adding bulk hotels.");
                return StatusCode(500, new { Message = "An error occurred while adding hotels in bulk." });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetHotelById(Guid id)
        {
            try
            {
                Log.Information("Fetching hotel details for HotelId: {HotelId}", id);
                var hotel = await _hotelService.GetHotelById(id);

                if (hotel == null)
                {
                    Log.Warning("Hotel not found: {HotelId}", id);
                    return NotFound(new { Message = "Hotel not found." });
                }

                return Ok(new { Message = "Hotel retrieved successfully.", Data = hotel });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching hotel details for HotelId: {HotelId}", id);
                return StatusCode(500, new { Message = "An error occurred while fetching the hotel details." });
            }
        }

        // TODO: Elasticsearch Search Feature
        // [HttpGet("search")]
        // public async Task<IActionResult> SearchHotels([FromQuery] string query)
        // {
        //     var searchResults = await _hotelService.SearchHotels(query);
        //     return Ok(searchResults);
        // }
    }
}
