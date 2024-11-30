using AutoMapper;
using HotelService.Data;
using HotelService.DTOs;
using HotelService.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace HotelService.Services
{
    public class HotelManagementService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConnection _rabbitConnection;

        public HotelManagementService(AppDbContext context, IMapper mapper, IConnection rabbitConnection)
        {
            _context = context;
            _mapper = mapper;
            _rabbitConnection = rabbitConnection;
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

            PublishHotelAddedEvent(hotel);
        }

        // 3. Otel Güncelleme
        public async Task UpdateHotel(Guid id, UpdateHotelDTO hotelDTO)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
                throw new Exception("Hotel not found");

            _mapper.Map(hotelDTO, hotel);
            await _context.SaveChangesAsync();

            PublishHotelUpdatedEvent(hotel);
        }

        // 4. Otel Silme
        public async Task DeleteHotel(Guid id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
                throw new Exception("Hotel not found");

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();

            PublishHotelDeletedEvent(id);
        }

        // RabbitMQ Eventleri Yayınlama Metotları
        private void PublishHotelAddedEvent(Hotel hotel)
        {
            PublishEvent("hotel-events", new HotelAddedEvent
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Location = hotel.Location
            });
        }

        private void PublishHotelUpdatedEvent(Hotel hotel)
        {
            PublishEvent("hotel-events", new HotelUpdatedEvent
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Location = hotel.Location
            });
        }

        private void PublishHotelDeletedEvent(Guid id)
        {
            PublishEvent("hotel-events", new HotelDeletedEvent
            {
                Id = id
            });
        }

        private void PublishEvent<T>(string queueName, T eventMessage)
        {
            using var channel = _rabbitConnection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            var message = JsonConvert.SerializeObject(eventMessage);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
            Console.WriteLine($"{typeof(T).Name} published: {message}");
        }
    }
}
