using Devkit.Common.Identity.Core.Interfaces;
using Devkit.Common.Identity.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Devkit.Common.Identity.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpPost] 
        public async Task<IActionResult> Create(CreateUserDto dto)
        {
            var id = await userService.CreateUserAsync(dto);
            return Ok(new { Id = id });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await userService.GetUserByIdAsync(id);
            return Ok(user);
        } 
    }
}