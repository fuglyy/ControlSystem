using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize] // все действия требуют авторизации
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    // Получить список всех пользователей
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAsync();
        return Ok(users.Select(u => new { u.Id, u.Username, u.Role }));
    }

    // Получить конкретного пользователя по ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _userService.GetByIdAsync(id);
        return user == null ? NotFound() : Ok(new { user.Id, user.Username, user.Role });
    }

    // Можно добавить смену роли (только для менеджера/админа)
    [HttpPut("{id}/role")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> ChangeRole(string id, [FromBody] ChangeRoleRequest req)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();

        user.Role = req.Role;
        await _userService.UpdateAsync(user);
        return Ok(new { user.Id, user.Username, user.Role });
    }
}

public record ChangeRoleRequest(string Role);
