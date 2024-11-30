using AutoMapper;
using HotelService.Data;
using HotelService.DTOs;
using HotelService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelService.Services
{
    public class HotelManagementService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public HotelManagementService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<HotelDTO>> GetAllHotels()
        {
            var hotels = await _context.Hotels.Include(h => h.Contacts).ToListAsync();
            return _mapper.Map<List<HotelDTO>>(hotels);
        }

        // 2. Yeni Otel Ekleme
        public async Task AddHotel(CreateHotelDTO hotelDTO)
        {
            var hotel = _mapper.Map<Hotel>(hotelDTO);
            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();
        }

        // 3. Otel Güncelleme
        public async Task UpdateHotel(Guid id, UpdateHotelDTO hotelDTO)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) throw new Exception("Hotel not found");

            _mapper.Map(hotelDTO, hotel);
            await _context.SaveChangesAsync();
        }

        // 4. Otel Silme
        public async Task DeleteHotel(Guid id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) throw new Exception("Hotel not found");

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
        }
    }
}
