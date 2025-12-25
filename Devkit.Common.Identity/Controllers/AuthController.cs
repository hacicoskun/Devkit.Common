using Devkit.Common.Identity.Core.Interfaces;
using Devkit.Common.Identity.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Devkit.Common.Identity.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        public AuthController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (!result.IsSuccess) return Unauthorized(result);
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            var result = await _authService.LogoutAsync(refreshToken);
            return result ? Ok(new { Message = "Logged out" }) : BadRequest("Logout failed");
        }
    }
}