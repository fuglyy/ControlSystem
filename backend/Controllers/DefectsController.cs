using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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
    [Authorize(Roles = "Engineer,Manager,Director")]
    public async Task<IActionResult> List() =>
        Ok(await _defectService.GetAllAsync());

    [HttpGet("{id}")]
    [Authorize(Roles = "Engineer,Manager,Director")]
    public async Task<IActionResult> Get(string id)
    {
        var d = await _defectService.GetByIdAsync(id);
        return d == null ? NotFound() : Ok(d);
    }

    [HttpPost]
    [Authorize(Roles = "Engineer")]
    public async Task<IActionResult> Create([FromBody] Defect d)
    {
        await _defectService.CreateAsync(d);
        return CreatedAtAction(nameof(Get), new { id = d.Id }, d);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Engineer,Manager")]
    public async Task<IActionResult> Update(string id, [FromBody] Defect d)
    {
        var exists = await _defectService.GetByIdAsync(id);
        if (exists == null) return NotFound();

        d.Id = id; 
        var ok = await _defectService.UpdateAsync(id, d);
        return ok ? NoContent() : StatusCode(500);
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Delete(string id)
    {
        var ok = await _defectService.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}
