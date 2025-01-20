using JwtAuthService.Models;
using JwtAuthService.Services;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthService.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] User user)
    {
        _authService.RegisterUser(user);
        return Ok(new { Message = "User registered successfully" });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] AuthRequest request)
    {
        try
        {
            var token = _authService.Authenticate(request.Username, request.Password);
            return Ok(new AuthResponse { Token = token });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { Message = "Invalid credentials" });
        }
    }
}