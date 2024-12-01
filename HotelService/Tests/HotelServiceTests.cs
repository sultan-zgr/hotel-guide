using AutoMapper;
using HotelService.Data;
using HotelService.DTOs.ContactDTOs;
using HotelService.DTOs.HotelDTOs;
using HotelService.Models;
using HotelService.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using shared.Messaging.RabbitMQ;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class HotelServiceTests
{
    private readonly AppDbContext _context;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRabbitMQPublisher> _mockPublisher;
    private readonly HotelManagementService _hotelService;
    private readonly ContactService _contactService;

    public HotelServiceTests()
    {
        // InMemoryDatabase ayarları
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("HotelTestDb")
            .Options;
        _context = new AppDbContext(options);

        // Mock bağımlılıklar
        _mockMapper = new Mock<IMapper>();
        _mockPublisher = new Mock<IRabbitMQPublisher>();

        // Servisler
        _hotelService = new HotelManagementService(_context, _mockMapper.Object, _mockPublisher.Object);
        _contactService = new ContactService(_context, _mockMapper.Object);
    }

    [Fact]
    public async Task AddHotel_ShouldAddHotelAndPublishEvent()
    {
        // Arrange
        var createHotelDTO = new CreateHotelDTO { Name = "New Hotel", Location = "New Location" };
        var hotel = new Hotel { Id = Guid.NewGuid(), Name = "New Hotel", Location = "New Location" };

        _mockMapper.Setup(m => m.Map<Hotel>(It.IsAny<CreateHotelDTO>())).Returns(hotel);

        // Act
        await _hotelService.AddHotel(createHotelDTO);

        // Assert
        var addedHotel = _context.Hotels.FirstOrDefault(h => h.Name == "New Hotel");
        Assert.NotNull(addedHotel);
        _mockPublisher.Verify(p => p.PublishHotelAddedEvent(It.IsAny<HotelAddedEvent>()), Times.Once);
    }

    [Fact]
    public async Task DeleteHotel_ShouldRemoveHotelAndPublishEvent()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        _context.Hotels.Add(new Hotel { Id = hotelId, Name = "Hotel to Delete", Location = "Location" });
        await _context.SaveChangesAsync();

        // Act
        await _hotelService.DeleteHotel(hotelId);

        // Assert
        var deletedHotel = _context.Hotels.FirstOrDefault(h => h.Id == hotelId);
        Assert.Null(deletedHotel);
        _mockPublisher.Verify(p => p.PublishHotelDeletedEvent(It.IsAny<HotelDeletedEvent>()), Times.Once);
    }

    [Fact]
    public async Task AddContact_ShouldAddContactToHotel()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        _context.Hotels.Add(new Hotel { Id = hotelId, Name = "Hotel with Contact", Location = "Location" });
        await _context.SaveChangesAsync();

        var createContactDTO = new CreateContactDTO { Type = "Phone", Value = "123456789" };
        var contact = new Contact { Id = Guid.NewGuid(), Type = "Phone", Value = "123456789", HotelId = hotelId };

        _mockMapper.Setup(m => m.Map<Contact>(It.IsAny<CreateContactDTO>())).Returns(contact);

        // Act
        await _contactService.AddContact(hotelId, createContactDTO);

        // Assert
        var addedContact = _context.Contacts.FirstOrDefault(c => c.Value == "123456789");
        Assert.NotNull(addedContact);
        Assert.Equal(hotelId, addedContact.HotelId);
        Assert.Equal("Phone", addedContact.Type);
        Assert.Equal("123456789", addedContact.Value);
    }

}
