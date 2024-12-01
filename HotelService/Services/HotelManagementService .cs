using AutoMapper;
using HotelService.Data;
using HotelService.DTOs.HotelDTOs;
using HotelService.Models;
using Microsoft.EntityFrameworkCore;
using shared.Messaging.RabbitMQ;

namespace HotelService.Services
{
    public class HotelManagementService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IRabbitMQPublisher _publisher;

        public HotelManagementService(AppDbContext context, IMapper mapper, IRabbitMQPublisher publisher)
        {
            _context = context;
            _mapper = mapper;
            _publisher = publisher;
        }

        // 1. Tüm Otelleri Listeleme
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

            _publisher.PublishHotelAddedEvent(new HotelAddedEvent
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Location = hotel.Location
            });
        }

        // 3. Otel Güncelleme
        public async Task UpdateHotel(Guid id, UpdateHotelDTO hotelDTO)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
                throw new Exception("Hotel not found");

            _mapper.Map(hotelDTO, hotel);
            await _context.SaveChangesAsync();

            _publisher.PublishHotelUpdatedEvent(new HotelUpdatedEvent
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Location = hotel.Location
            });
        }

        // 4. Otel Silme
        public async Task DeleteHotel(Guid id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
                throw new Exception("Hotel not found");

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();

            _publisher.PublishHotelDeletedEvent(new HotelDeletedEvent
            {
                Id = id
            });
        }
    }
}
