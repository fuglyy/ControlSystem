using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class DefectsController : ControllerBase
{
    private readonly IDefectService _defectService;

    public DefectsController(IDefectService defectService)
    {
        _defectService = defectService;
    }

   
    [HttpGet]
    [Authorize(Roles = "Engineer,Manager,Director")]
    public async Task<ActionResult<IEnumerable<DefectSummaryDto>>> List()
    {
        return Ok(await _defectService.GetDefectListAsync());  
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Engineer,Manager,Director")]
    public async Task<ActionResult<DefectDetailDto>> Get(string id)
    {
        var defectDto = await _defectService.GetDefectDetailsAsync(id);

        if (defectDto == null)
        {
            return NotFound();
        }

        return defectDto;
    }

    [HttpPost]
    [Authorize(Roles = "Engineer")]
    public async Task<IActionResult> Create([FromBody] Defect d)
    {
        d.CreatedAt = DateTime.UtcNow; 
        
        await _defectService.CreateAsync(d);
        
        return CreatedAtAction(nameof(Get), new { id = d.Id }, new { id = d.Id });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Engineer,Manager")]
    public async Task<IActionResult> Update(string id, [FromBody] Defect d)
    {
        
        d.Id = id; 
        var ok = await _defectService.UpdateAsync(id, d);
        
        return ok ? NoContent() : NotFound(); 
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Delete(string id)
    {
        var ok = await _defectService.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}