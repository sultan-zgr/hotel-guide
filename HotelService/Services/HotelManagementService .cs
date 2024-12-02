using AutoMapper;
using HotelService.Data;
using HotelService.DTOs.HotelDTOs;
using HotelService.Models;
using Microsoft.EntityFrameworkCore;
using shared.Messaging.RabbitMQ;
using AutoMapper.QueryableExtensions;

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
            return await _context.Hotels
                .Include(h => h.Contacts)
                .ProjectTo<HotelDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // 2. Yeni Otel Ekleme
        public async Task AddHotel(CreateHotelDTO hotelDTO)
        {
            var hotel = _mapper.Map<Hotel>(hotelDTO);
            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();

            var hotelAddedEvent = _mapper.Map<HotelAddedEvent>(hotel);
            _publisher.PublishHotelAddedEvent(hotelAddedEvent);
        }

        // 3. Otel Güncelleme
        public async Task UpdateHotel(Guid id, UpdateHotelDTO hotelDTO)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
                throw new Exception("Hotel not found");

            _mapper.Map(hotelDTO, hotel);
            await _context.SaveChangesAsync();

            var hotelUpdatedEvent = _mapper.Map<HotelUpdatedEvent>(hotel);
            _publisher.PublishHotelUpdatedEvent(hotelUpdatedEvent);
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

        // 5. Toplu Otel Ekleme
        public async Task AddHotelsBulk(List<CreateHotelDTO> hotelsDTO)
        {
            var hotels = _mapper.Map<List<Hotel>>(hotelsDTO);
            _context.Hotels.AddRange(hotels);
            await _context.SaveChangesAsync();

            foreach (var hotel in hotels)
            {
                var hotelAddedEvent = _mapper.Map<HotelAddedEvent>(hotel);
                _publisher.PublishHotelAddedEvent(hotelAddedEvent);
            }
        }
    }
}