using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Roles = "Manager, Director")]
    public async Task<ActionResult<IEnumerable<UserSummaryDto>>> GetAll()
    {
        var users = await _userService.GetAsync();
        
        // ⭐ Используем LINQ для маппинга в DTO (убираем анонимные объекты) ⭐
        var userDtos = users.Select(u => new UserSummaryDto { 
            Id = u.Id, 
            Username = u.Username, 
            Role = u.Role 
        });
        
        return Ok(userDtos);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Manager,Engineer,Director")]
    public async Task<ActionResult<UserSummaryDto>> GetById(string id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();
        
        // ⭐ Маппинг в DTO ⭐
        var userDto = new UserSummaryDto {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role
        };
        return Ok(userDto);
    }

    [HttpPut("{id}/role")]
    [Authorize(Roles = "Manager")]
    public async Task<ActionResult<UserSummaryDto>> ChangeRole(string id, [FromBody] ChangeRoleRequest req)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();

        user.Role = req.Role;
        await _userService.UpdateAsync(user);
        
        // ⭐ Маппинг в DTO перед возвратом ⭐
        var userDto = new UserSummaryDto {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role
        };
        return Ok(userDto);
    }
}

public record ChangeRoleRequest(string Role);