using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DefectsController : ControllerBase
{
    private readonly DefectService _defectService;

    public DefectsController(DefectService defectService)
    {
        _defectService = defectService;
    }

    [HttpGet]
    public async Task<IActionResult> List() =>
        Ok(await _defectService.GetAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var d = await _defectService.GetAsync(id);
        return d == null ? NotFound() : Ok(d);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Defect d)
    {
        await _defectService.CreateAsync(d);
        return CreatedAtAction(nameof(Get), new { id = d.Id }, d);
    }
}
