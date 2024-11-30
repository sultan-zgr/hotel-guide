using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using ReportService.Data;
using ReportService.DTOs;
using ReportService.Models;
using System.Text;

namespace ReportService.Services
{
    public class ReportManagementService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConnection _connection;

        public ReportManagementService(AppDbContext context, IMapper mapper, IConnection connection)
        {
            _context = context;
            _mapper = mapper;
            _connection = connection;
        }

        // Tüm raporları getir
        public async Task<IEnumerable<ReportDTO>> GetAllReports()
        {
            var reports = await _context.Reports.ToListAsync();
            return _mapper.Map<IEnumerable<ReportDTO>>(reports);
        }

        // Belirli bir raporun detayını getir
        public async Task<ReportDTO> GetReportById(Guid id)
        {
            var report = await _context.Reports.FindAsync(id);
            return _mapper.Map<ReportDTO>(report);
        }

        // RabbitMQ kuyruğuna rapor talebi ekle
        public void QueueReportRequest(CreateReportRequestDTO reportRequestDTO)
        {
            var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: "report-queue", durable: true, exclusive: false, autoDelete: false);

            var message = JsonConvert.SerializeObject(reportRequestDTO);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: "report-queue", basicProperties: null, body: body);
        }
    }
}
