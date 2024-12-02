using HotelService.DTOs.ContactDTOs;
using HotelService.Services;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Serilog;

namespace HotelService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly ContactService _contactService;
        private readonly IValidator<CreateContactDTO> _contactValidator;

        public ContactController(ContactService contactService, IValidator<CreateContactDTO> contactValidator)
        {
            _contactService = contactService;
            _contactValidator = contactValidator;
        }

        [HttpPost("{hotelId:guid}/contacts")]
        public async Task<IActionResult> AddContact(Guid hotelId, [FromBody] CreateContactDTO contactDTO)
        {
            var validationResult = await _contactValidator.ValidateAsync(contactDTO);

            if (!validationResult.IsValid)
            {
                Log.Warning("Validation failed for ContactDTO: {Errors}", validationResult.Errors);
                return BadRequest(validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
            }

            try
            {
                await _contactService.AddContact(hotelId, contactDTO);
                Log.Information("Contact added for HotelId: {HotelId}", hotelId);
                return Created("", new { Message = "Contact added successfully." });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error adding contact for HotelId: {HotelId}", hotelId);
                return StatusCode(500, new { Message = "An error occurred while adding the contact." });
            }
        }

        [HttpDelete("contacts/{id:guid}")]
        public async Task<IActionResult> DeleteContact(Guid id)
        {
            try
            {
                await _contactService.DeleteContact(id);
                Log.Information("Contact deleted with Id: {ContactId}", id);
                return Ok(new { Message = "Contact deleted successfully." });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting contact with Id: {ContactId}", id);
                return StatusCode(500, new { Message = "An error occurred while deleting the contact." });
            }
        }
    }
}
