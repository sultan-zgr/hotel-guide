using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using RabbitMQ.Client;
using ReportService.Data;
using ReportService.DTOs;
using ReportService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class ReportServiceTests
{
    private readonly AppDbContext _context;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ReportManagementService _service;
    private readonly Mock<IConnection> _mockConnection;
    public ReportServiceTests()
    {
        // InMemoryDatabase ayarları
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("ReportTestDb")
            .Options;
        _context = new AppDbContext(options);

        // Mock bağımlılıklar
        _mockMapper = new Mock<IMapper>();
        _mockConnection = new Mock<IConnection>();

        // Servis
        _service = new ReportManagementService(_context, _mockConnection.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetReportById_ShouldReturnReport_WhenReportExists()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var report = new Report
        {
            Id = reportId,
            Location = "Test Location",
            HotelCount = 10,
            ContactCount = 20,
            Status = "Preparing",
            RequestedAt = DateTime.UtcNow
        };
        _context.Reports.Add(report);
        await _context.SaveChangesAsync();

        var reportDTO = new ReportDTO
        {
            Id = reportId,
            Location = "Test Location",
            HotelCount = 10,
            ContactCount = 20,
            Status = "Preparing",
            RequestedAt = report.RequestedAt
        };
        _mockMapper.Setup(m => m.Map<ReportDTO>(It.IsAny<Report>())).Returns(reportDTO);

        // Act
        var result = await _service.GetReportById(reportId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(reportId, result.Id);
        Assert.Equal("Test Location", result.Location);
        Assert.Equal(10, result.HotelCount);
        Assert.Equal(20, result.ContactCount);
        Assert.Equal("Preparing", result.Status);
        Assert.Equal(report.RequestedAt, result.RequestedAt);
    }

    [Fact]
    public async Task GetReportById_ShouldReturnNull_WhenReportDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _service.GetReportById(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllReports_ShouldReturnAllReports()
    {
        // Arrange
        var reports = new List<Report>
        {
            new Report
            {
                Id = Guid.NewGuid(),
                Location = "Location 1",
                HotelCount = 5,
                ContactCount = 10,
                Status = "Completed",
                RequestedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Report
            {
                Id = Guid.NewGuid(),
                Location = "Location 2",
                HotelCount = 8,
                ContactCount = 15,
                Status = "Preparing",
                RequestedAt = DateTime.UtcNow
            }
        };
        _context.Reports.AddRange(reports);
        await _context.SaveChangesAsync();

        var reportDTOs = reports.Select(r => new ReportDTO
        {
            Id = r.Id,
            Location = r.Location,
            HotelCount = r.HotelCount,
            ContactCount = r.ContactCount,
            Status = r.Status,
            RequestedAt = r.RequestedAt
        }).ToList();

        _mockMapper.Setup(m => m.Map<List<ReportDTO>>(It.IsAny<List<Report>>())).Returns(reportDTOs);

        // Act
        var result = await _service.GetAllReports();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Location 1", result[0].Location);
        Assert.Equal("Location 2", result[1].Location);
    }
}
