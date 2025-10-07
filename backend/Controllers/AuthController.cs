using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase {
    private readonly IUserService _userService;
    private readonly IConfiguration _config;

    public AuthController(IUserService userService, IConfiguration config){
        _userService = userService;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserSummaryDto>> Register([FromBody] RegisterRequest req){ // Изменили тип
        var exists = await _userService.GetByUsernameAsync(req.Username);
        if (exists != null) return BadRequest("User exists");

        var user = new User{
            Username = req.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role = req.Role ?? "Engineer"
        };

        await _userService.CreateAsync(user);
        
        // ⭐ Маппинг в DTO перед возвратом (убираем анонимный объект) ⭐
        var userDto = new UserSummaryDto {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role
        };
        return Ok(userDto); 
    }
    
    // ... (Login и GenerateJwtToken остаются без изменений) ...
}

public record RegisterRequest(string Username, string Password, string? Role);
public record LoginRequest(string Username, string Password);