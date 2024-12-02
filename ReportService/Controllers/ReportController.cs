using Microsoft.AspNetCore.Mvc;
using ReportService.DTOs;
using ReportService.Services;
using FluentValidation;
using FluentValidation.Results;
using Serilog;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly ReportManagementService _reportService;
    private readonly IValidator<CreateReportRequestDTO> _createReportRequestValidator;

    public ReportController(
        ReportManagementService reportService,
        IValidator<CreateReportRequestDTO> createReportRequestValidator)
    {
        _reportService = reportService;
        _createReportRequestValidator = createReportRequestValidator;
    }

    // 1. Rapor Talebi Oluştur
    [HttpPost]
    public async Task<IActionResult> CreateReportRequest([FromBody] CreateReportRequestDTO reportRequestDTO)
    {
        ValidationResult validationResult = await _createReportRequestValidator.ValidateAsync(reportRequestDTO);

        if (!validationResult.IsValid)
        {
            Log.Warning("Validation failed for CreateReportRequestDTO: {Errors}", validationResult.Errors);
            return BadRequest(new
            {
                Message = "Validation failed for the report request.",
                Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }

        try
        {
            await _reportService.QueueReportRequest(reportRequestDTO);
            Log.Information("Report request has been queued successfully for location: {Location}", reportRequestDTO.Location);
            return Accepted(new { Message = "Report request has been queued successfully." });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error queuing report request for location: {Location}", reportRequestDTO.Location);
            return StatusCode(500, new { Message = "An error occurred while queuing the report request." });
        }
    }

    // 2. Rapor ID'ye Göre Getir
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetReportById(Guid id)
    {
        try
        {
            Log.Information("Fetching report details for ReportId: {ReportId}", id);
            var report = await _reportService.GetReportById(id);

            if (report == null)
            {
                Log.Warning("Report not found: {ReportId}", id);
                return NotFound(new { Message = "Report not found." });
            }

            Log.Information("Report retrieved successfully for ReportId: {ReportId}", id);
            return Ok(new { Message = "Report retrieved successfully.", Data = report });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error fetching report for ReportId: {ReportId}", id);
            return StatusCode(500, new { Message = "An error occurred while fetching the report details." });
        }
    }

    // 3. Tüm Raporları Listele
    [HttpGet]
    public async Task<IActionResult> GetAllReports()
    {
        try
        {
            Log.Information("Fetching all reports.");
            var reportDTOs = await _reportService.GetAllReports();
            return Ok(new { Message = "Reports retrieved successfully.", Data = reportDTOs });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error fetching all reports.");
            return StatusCode(500, new { Message = "An error occurred while fetching the reports." });
        }
    }
}
