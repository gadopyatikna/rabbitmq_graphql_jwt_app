using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthService.Controllers;

[ApiController]
[Route("[controller]")]
public class ProtectedController : ControllerBase
{
    [HttpGet("protected")]
    [Authorize]
    public IActionResult Protected()
    {
        return Ok(new { Message = "This is a protected endpoint" });
    }
}