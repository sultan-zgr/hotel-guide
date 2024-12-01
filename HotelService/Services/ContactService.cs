using AutoMapper;
using HotelService.Data;
using HotelService.DTOs.ContactDTOs;
using HotelService.Models;

namespace HotelService.Services
{
    public class ContactService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ContactService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // 1. İletişim Bilgisi Ekleme
        public async Task AddContact(Guid hotelId, CreateContactDTO contactDTO)
        {
            var hotel = await _context.Hotels.FindAsync(hotelId);
            if (hotel == null) throw new Exception("Hotel not found");

            var contact = _mapper.Map<Contact>(contactDTO);
            contact.HotelId = hotelId;
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
        }

        // 2. İletişim Bilgisi Silme
        public async Task DeleteContact(Guid contactId)
        {
            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact == null) throw new Exception("Contact not found");

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
        }
    }

}
