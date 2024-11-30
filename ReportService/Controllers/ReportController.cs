using Microsoft.AspNetCore.Mvc;
using ReportService.DTOs;
using ReportService.Models;
using ReportService.Services;

namespace ReportService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ReportManagementService _reportService;

        public ReportController(ReportManagementService reportService)
        {
            _reportService = reportService;
        }

        // 1. Tüm raporları listeleme
        [HttpGet]
        public async Task<IActionResult> GetAllReports()
        {
            var reports = await _reportService.GetAllReports();
            return Ok(reports);
        }

        // 2. Belirli bir raporun detayını getirme
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReportById(Guid id)
        {
            var report = await _reportService.GetReportById(id);
            if (report == null)
            {
                return NotFound(new { message = "Report not found." });
            }
            return Ok(report);
        }

        // 3. Yeni rapor talebi oluşturma
        [HttpPost]
        public IActionResult CreateReportRequest([FromBody] CreateReportRequestDTO reportRequestDTO)
        {
            // Rapor talebini RabbitMQ kuyruğuna gönder
            _reportService.QueueReportRequest(reportRequestDTO);
            return Accepted(new { message = "Report request has been queued successfully." });
        }
    }
}
