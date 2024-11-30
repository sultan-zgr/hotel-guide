using HotelService.DTOs;
using HotelService.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelService.Controllers
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

        [HttpPost]
        public IActionResult RequestReport([FromBody] CreateReportRequestDTO reportRequestDTO)
        {
            _reportService.RequestReport(reportRequestDTO);
            return Accepted(); // Request accepted for asynchronous processing
        }
    }
}
