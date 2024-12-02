using HotelService.Data;
using HotelService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelService.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly AppDbContext _context;

        public HotelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Hotel>> GetAllHotelsAsync()
        {
            return await _context.Hotels
                .Include(h => h.Contacts)
                .ToListAsync();
        }

        public async Task<Hotel> GetHotelByIdAsync(Guid id)
        {
            return await _context.Hotels
                .Include(h => h.Contacts)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task AddHotelAsync(Hotel hotel)
        {
            await _context.Hotels.AddAsync(hotel);
            await SaveChangesAsync();
        }

        public async Task AddHotelRangeAsync(List<Hotel> hotels)
        {
            await _context.Hotels.AddRangeAsync(hotels);
            await SaveChangesAsync();
        }

        public async Task UpdateHotelAsync(Hotel hotel)
        {
            _context.Hotels.Update(hotel);
            await SaveChangesAsync();
        }

        public async Task DeleteHotelAsync(Hotel hotel)
        {
            _context.Hotels.Remove(hotel);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
