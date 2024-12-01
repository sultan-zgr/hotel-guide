using HotelService.Models;

public interface IHotelRepository
{
    Task<List<Hotel>> GetAllHotelsAsync();
    Task<Hotel> GetHotelByIdAsync(Guid id);
    Task AddHotelAsync(Hotel hotel);
    Task UpdateHotelAsync(Hotel hotel);
    Task DeleteHotelAsync(Guid id);
}
