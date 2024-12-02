using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using HotelService.DTOs.HotelDTOs;
using HotelService.Models;
using HotelService.Services;
using Moq;
using shared.Messaging.RabbitMQ;
using Xunit;

namespace HotelService.Tests
{
    public class HotelManagementServiceTests
    {
        private readonly Mock<IHotelRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IRabbitMQPublisher> _mockPublisher;
        private readonly HotelManagementService _service;

        public HotelManagementServiceTests()
        {
            _mockRepository = new Mock<IHotelRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockPublisher = new Mock<IRabbitMQPublisher>();
            _service = new HotelManagementService(
                _mockRepository.Object,
                _mockMapper.Object,
                _mockPublisher.Object
            );
        }

        [Fact]
        public async Task GetAllHotels_ReturnsListOfHotelDTOs()
        {
            // Arrange
            var hotels = new List<Hotel>
            {
                new Hotel { Id = Guid.NewGuid(), Name = "Hotel 1", Location = "Location 1" },
                new Hotel { Id = Guid.NewGuid(), Name = "Hotel 2", Location = "Location 2" }
            };
            var hotelDTOs = new List<HotelDTO>
            {
                new HotelDTO { Name = "Hotel 1", Location = "Location 1" },
                new HotelDTO { Name = "Hotel 2", Location = "Location 2" }
            };

            _mockRepository.Setup(r => r.GetAllHotelsAsync()).ReturnsAsync(hotels);
            _mockMapper.Setup(m => m.Map<List<HotelDTO>>(hotels)).Returns(hotelDTOs);

            // Act
            var result = await _service.GetAllHotels();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Hotel 1", result[0].Name);
            Assert.Equal("Hotel 2", result[1].Name);
        }

        [Fact]
        public async Task AddHotel_ValidHotel_AddsHotelAndPublishesEvent()
        {
            // Arrange
            var createHotelDTO = new CreateHotelDTO
            {
                Name = "New Hotel",
                Location = "Test Location"
            };
            var hotel = new Hotel
            {
                Id = Guid.NewGuid(),
                Name = "New Hotel",
                Location = "Test Location"
            };
            var hotelAddedEvent = new HotelAddedEvent
            {
                Id = hotel.Id,
                Name = "New Hotel"
            };

            _mockMapper.Setup(m => m.Map<Hotel>(createHotelDTO)).Returns(hotel);
            _mockMapper.Setup(m => m.Map<HotelAddedEvent>(hotel)).Returns(hotelAddedEvent);

            // Act
            await _service.AddHotel(createHotelDTO);

            // Assert
            _mockRepository.Verify(r => r.AddHotelAsync(hotel), Times.Once);
            _mockPublisher.Verify(p => p.PublishHotelAddedEvent(hotelAddedEvent), Times.Once);
        }

        [Fact]
        public async Task UpdateHotel_ExistingHotel_UpdatesHotelAndPublishesEvent()
        {
            // Arrange
            var hotelId = Guid.NewGuid();
            var existingHotel = new Hotel
            {
                Id = hotelId,
                Name = "Old Hotel Name"
            };
            var updateHotelDTO = new UpdateHotelDTO
            {
                Name = "Updated Hotel Name"
            };
            var hotelUpdatedEvent = new HotelUpdatedEvent
            {
                Id = hotelId,
                Name = "Updated Hotel Name"
            };

            _mockRepository.Setup(r => r.GetHotelByIdAsync(hotelId)).ReturnsAsync(existingHotel);
            _mockMapper.Setup(m => m.Map(updateHotelDTO, existingHotel)).Callback<UpdateHotelDTO, Hotel>((dto, hotel) =>
            {
                hotel.Name = dto.Name;
            });
            _mockMapper.Setup(m => m.Map<HotelUpdatedEvent>(existingHotel)).Returns(hotelUpdatedEvent);

            // Act
            await _service.UpdateHotel(hotelId, updateHotelDTO);

            // Assert
            _mockRepository.Verify(r => r.UpdateHotelAsync(existingHotel), Times.Once);
            _mockPublisher.Verify(p => p.PublishHotelUpdatedEvent(hotelUpdatedEvent), Times.Once);
            Assert.Equal("Updated Hotel Name", existingHotel.Name);
        }

        [Fact]
   
        public async Task DeleteHotel_ExistingHotel_DeletesHotelAndPublishesEvent()
        {
            // Arrange
            var hotelId = Guid.NewGuid();
            var existingHotel = new Hotel
            {
                Id = hotelId,
                Name = "Hotel to Delete"
            };

            _mockRepository.Setup(r => r.GetHotelByIdAsync(hotelId)).ReturnsAsync(existingHotel);

            // Act
            await _service.DeleteHotel(hotelId);

            // Assert
            _mockRepository.Verify(r => r.DeleteHotelAsync(It.Is<Hotel>(h => h.Id == hotelId)), Times.Once);
            _mockPublisher.Verify(p => p.PublishHotelDeletedEvent(
                It.Is<HotelDeletedEvent>(e => e.Id == hotelId)), Times.Once);
        }


        [Fact]
        public async Task DeleteHotel_NonExistentHotel_ThrowsException()
        {
            // Arrange
            var hotelId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetHotelByIdAsync(hotelId)).ReturnsAsync((Hotel)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.DeleteHotel(hotelId));
            Assert.Equal("Hotel not found", exception.Message);
        }
    }
}
