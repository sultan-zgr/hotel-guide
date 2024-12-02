using AutoMapper;
using HotelService.DTOs.HotelDTOs;
using HotelService.Models;
using shared.Messaging.RabbitMQ;

public class HotelManagementService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IMapper _mapper;
    private readonly IRabbitMQPublisher _publisher;

    public HotelManagementService(
        IHotelRepository hotelRepository,
        IMapper mapper,
        IRabbitMQPublisher publisher)
    {
        _hotelRepository = hotelRepository;
        _mapper = mapper;
        _publisher = publisher;
    }

    // 1. Tüm Otelleri Listeleme
    public async Task<List<HotelDTO>> GetAllHotels()
    {
        var hotels = await _hotelRepository.GetAllHotelsAsync();
        return _mapper.Map<List<HotelDTO>>(hotels);
    }

    // 2. Yeni Otel Ekleme
    public async Task AddHotel(CreateHotelDTO hotelDTO)
    {
        var hotel = _mapper.Map<Hotel>(hotelDTO);
        await _hotelRepository.AddHotelAsync(hotel);

        var hotelAddedEvent = _mapper.Map<HotelAddedEvent>(hotel);
        _publisher.PublishHotelAddedEvent(hotelAddedEvent);
    }

    // 3. Otel Güncelleme
    public async Task UpdateHotel(Guid id, UpdateHotelDTO hotelDTO)
    {
        var hotel = await _hotelRepository.GetHotelByIdAsync(id);
        if (hotel == null)
            throw new Exception("Hotel not found");

        _mapper.Map(hotelDTO, hotel);
        await _hotelRepository.UpdateHotelAsync(hotel);

        var hotelUpdatedEvent = _mapper.Map<HotelUpdatedEvent>(hotel);
        _publisher.PublishHotelUpdatedEvent(hotelUpdatedEvent);
    }
    // 4. Otel Silme
    public async Task DeleteHotel(Guid id)
    {
        var hotel = await _hotelRepository.GetHotelByIdAsync(id);
        if (hotel == null)
            throw new Exception("Hotel not found");

        await _hotelRepository.DeleteHotelAsync(hotel);

        _publisher.PublishHotelDeletedEvent(new HotelDeletedEvent
        {
            Id = id
        });
    }

    // 5. Toplu Otel Ekleme
    public async Task AddHotelsBulk(List<CreateHotelDTO> hotelsDTO)
    {
        var hotels = _mapper.Map<List<Hotel>>(hotelsDTO);
        await _hotelRepository.AddHotelRangeAsync(hotels);

        foreach (var hotel in hotels)
        {
            var hotelAddedEvent = _mapper.Map<HotelAddedEvent>(hotel);
            _publisher.PublishHotelAddedEvent(hotelAddedEvent);
        }
    }
    // 6. Otel ID'sine Göre Getirme
    public async Task<HotelDTO> GetHotelById(Guid id)
    {
        var hotel = await _hotelRepository.GetHotelByIdAsync(id);
        if (hotel == null)
            throw new Exception("Hotel not found");

        return _mapper.Map<HotelDTO>(hotel);
    }

}
