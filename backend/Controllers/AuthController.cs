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
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _userService.GetByUsernameAsync(req.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Unauthorized();

        var token = GenerateJwtToken(user);
        return Ok(new { token });
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddDays(7);

        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record RegisterRequest(string Username, string Password, string? Role);
public record LoginRequest(string Username, string Password);