using HotelService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }

}
