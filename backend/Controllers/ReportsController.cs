using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    // ⭐ ИСПОЛЬЗУЕМ ТОЛЬКО IReportService ⭐
    private readonly IReportService _reportService; 

    // Обновлённый конструктор
    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    // 1. GET projects
    // Теперь вся логика находится в сервисе.
    [HttpGet("projects")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> ProjectReport()
    {
        return Ok(await _reportService.GetProjectReportAsync());
    }

    // 2. GET defects
    [HttpGet("defects")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> DefectReport()
    {
        return Ok(await _reportService.GetDefectReportAsync());
    }
    
    // 3. GET summary
    [HttpGet("summary")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Summary()
    {
        return Ok(await _reportService.GetSummaryReportAsync());
    }
}