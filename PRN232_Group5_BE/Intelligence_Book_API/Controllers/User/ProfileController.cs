using BLL.Services.Interfaces;
using BLL.Services.User.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static DAL.DTOs.UserAccount.AuthDTO;

namespace Intelligence_Book_API.Controllers.User
{
    [Route("api/auth/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IAuthService _authService;

        public ProfileController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var profile = await _authService.GetProfileAsync(username);
            if (profile == null) return NotFound("User not found.");

            return Ok(profile);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDTO request)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var success = await _authService.UpdateProfileAsync(username, request);
            if (!success) return BadRequest("Failed to update profile.");

            return Ok(new { message = "Profile updated successfully." });
        }
    }
}
