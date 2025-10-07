using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _service;

    public ProjectsController(IProjectService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "Engineer,Manager,Director")] // Добавил Authorize
    public async Task<IEnumerable<ProjectSummaryDto>> GetAll() => // Изменили тип
        await _service.GetAllSummariesAsync(); // Требуется новый метод в IProjectService

    [HttpGet("{id}")]
    [Authorize(Roles = "Engineer,Manager,Director")] // Добавил Authorize
    public async Task<ActionResult<ProjectDetailDto>> Get(string id) // Изменили тип
    {
        var projDto = await _service.GetDetailByIdAsync(id); // Требуется новый метод
        if (projDto == null) return NotFound();
        return projDto;
    }

    [HttpPost]
    [Authorize(Roles = "Manager,Admin")] 
    public async Task<IActionResult> Create(Project project)
    {
        project.CreatedAt = DateTime.UtcNow; // Установим дату создания на сервере
        await _service.CreateAsync(project);
        
        return CreatedAtAction(nameof(Get), new { id = project.Id }, new { id = project.Id }); 
    }

    [HttpPut("{id}/stage")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> AddStage(string id, [FromBody] ProjectStage stage)
    {
        var updated = await _service.AddStageAsync(id, stage);
        if (!updated) return NotFound();
        return NoContent();
    }
}