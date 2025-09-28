using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ProjectService _projects;
    private readonly DefectService _defects;

    public ReportsController(ProjectService projects, DefectService defects)
    {
        _projects = projects;
        _defects = defects;
    }

    [HttpGet("projects")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> ProjectReport()
    {
        var projects = await _projects.GetAllAsync();
        var report = projects.Select(p => new
        {
            Project = p.Name,
            TotalStages = p.Stages.Count,
            Completed = p.Stages.Count(s => s.Status == "Done"),
            InProgress = p.Stages.Count(s => s.Status == "InProgress"),
            Planned = p.Stages.Count(s => s.Status == "Planned")
        });

        return Ok(report);
    }

    [HttpGet("defects")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> DefectReport()
    {
        var defects = await _defects.GetAllAsync();
        var grouped = defects.GroupBy(d => d.Status).Select(g => new
        {
            Status = g.Key,
            Count = g.Count()
        });

        return Ok(grouped);
    }

    [HttpGet("summary")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Summary()
    {
        var projects = await _projects.GetAllAsync();
        var defects = await _defects.GetAllAsync();

        var summary = new
        {
            TotalProjects = projects.Count,
            TotalDefects = defects.Count,
            DefectsByStatus = defects.GroupBy(d => d.Status).Select(g => new { Status = g.Key, Count = g.Count() }),
            ProjectsProgress = projects.Select(p => new
            {
                p.Name,
                CompletedStages = p.Stages.Count(s => s.Status == "Done"),
                TotalStages = p.Stages.Count
            })
        };

        return Ok(summary);
    }
}
