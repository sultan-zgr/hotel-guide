using HotelService.Models;

public interface IHotelRepository
{
    Task<List<Hotel>> GetAllHotelsAsync();
    Task<Hotel> GetHotelByIdAsync(Guid id);
    Task AddHotelAsync(Hotel hotel);
    Task AddHotelRangeAsync(List<Hotel> hotels);
    Task UpdateHotelAsync(Hotel hotel);
    Task DeleteHotelAsync(Hotel hotel);
    Task SaveChangesAsync();
}
