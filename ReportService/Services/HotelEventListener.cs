using AutoMapper;
using ReportService.Data;
using ReportService.Models;
using shared.Messaging.RabbitMQ;

namespace ReportService.Services
{
    public class HotelEventListener
    {
        private readonly IRabbitMQSubscriber _subscriber;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public HotelEventListener(IRabbitMQSubscriber subscriber, IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _subscriber = subscriber;
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        public void StartListening()
        {
            _subscriber.Subscribe<HotelUpdatedEvent>("hotel-events", async hotelEvent =>
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var existingHotel = await dbContext.Hotels.FindAsync(hotelEvent.Id);
                if (existingHotel == null)
                {
                    var newHotel = _mapper.Map<Hotel>(hotelEvent);
                    dbContext.Hotels.Add(newHotel);
                }
                else
                {
                    _mapper.Map(hotelEvent, existingHotel);
                    dbContext.Hotels.Update(existingHotel);
                }

                await dbContext.SaveChangesAsync();
            });

            _subscriber.Subscribe<HotelDeletedEvent>("hotel-events", async hotelDeletedEvent =>
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var existingHotel = await dbContext.Hotels.FindAsync(hotelDeletedEvent.Id);
                if (existingHotel != null)
                {
                    dbContext.Hotels.Remove(existingHotel);
                    await dbContext.SaveChangesAsync();
                }
            });
        }
    }
}
