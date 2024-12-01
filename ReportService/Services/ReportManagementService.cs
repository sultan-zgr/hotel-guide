using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using ReportService.Data;
using ReportService.DTOs;
using ReportService.Models;
using System.Text;

public class ReportManagementService
{
    private readonly AppDbContext _context;
    private readonly IConnection _connection;
    private readonly IMapper _mapper;

    public ReportManagementService(AppDbContext context, IConnection connection, IMapper mapper)
    {
        _context = context;
        _connection = connection;
        _mapper = mapper;
    }

    public async Task QueueReportRequest(CreateReportRequestDTO reportRequestDTO)
    {
        // 1. Raporu "Preparing" durumu ile kaydet
        var report = _mapper.Map<Report>(reportRequestDTO);
        report.Status = "Preparing"; // Varsayılan durum
        report.RequestedAt = DateTime.UtcNow;

        _context.Reports.Add(report);
        await _context.SaveChangesAsync();

        // 2. RabbitMQ kuyruğuna mesaj gönder
        var channel = _connection.CreateModel();
        channel.QueueDeclare(queue: "report-queue", durable: true, exclusive: false, autoDelete: false);

        var message = JsonConvert.SerializeObject(new { reportId = report.Id, location = report.Location });
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "", routingKey: "report-queue", basicProperties: null, body: body);
    }

    public async Task<ReportDTO> GetReportById(Guid id)
    {
        var report = await _context.Reports.FindAsync(id);
        return _mapper.Map<ReportDTO>(report);
    }

    public async Task<List<ReportDTO>> GetAllReports()
    {
        var reports = await _context.Reports.ToListAsync();
        return _mapper.Map<List<ReportDTO>>(reports);
    }

}
