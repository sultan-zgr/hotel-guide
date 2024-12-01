using Microsoft.AspNetCore.Mvc;
using ReportService.DTOs;
using ReportService.Services;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly ReportManagementService _reportService;

    public ReportController(ReportManagementService reportService)
    {
        _reportService = reportService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReportRequest([FromBody] CreateReportRequestDTO reportRequestDTO)
    {
        await _reportService.QueueReportRequest(reportRequestDTO);
        return Accepted(new { message = "Report request has been queued successfully." });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReportById(Guid id)
    {
        var report = await _reportService.GetReportById(id);
        if (report == null) return NotFound(new { message = "Report not found." });

        return Ok(report);
    }
    [HttpGet]
    public async Task<IActionResult> GetAllReports()
    {
        var reportDTOs = await _reportService.GetAllReports();
        return Ok(reportDTOs);
    }

}
