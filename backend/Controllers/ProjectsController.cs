using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly ProjectService _service;

    public ProjectsController(ProjectService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IEnumerable<Project>> GetAll() =>
        await _service.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> Get(string id)
    {
        var proj = await _service.GetByIdAsync(id);
        if (proj == null) return NotFound();
        return proj;
    }

    [HttpPost]
    [Authorize(Roles = "Manager,Admin")] 
    public async Task<IActionResult> Create(Project project)
    {
        await _service.CreateAsync(project);
        return CreatedAtAction(nameof(Get), new { id = project.Id }, project);
    }

    [HttpPut("{id}/stage")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> AddStage(string id, ProjectStage stage)
    {
        var updated = await _service.AddStageAsync(id, stage);
        if (!updated) return NotFound();
        return NoContent();
    }
}
