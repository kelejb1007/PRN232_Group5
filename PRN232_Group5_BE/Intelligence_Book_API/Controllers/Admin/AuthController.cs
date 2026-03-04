using BLL.Services.Admin.Interfaces;
using DAL.DTOs.UserAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DAL.DTOs.UserAccount.AuthDTO;

namespace Intelligence_Book_API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private const string AuthCookieName = "access_token";
        public AuthController(IAuthService authService) => _authService = authService;



        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("UserName and Password are required.");
            }

            var authResult = await _authService.LoginAsync(request);
            if (authResult == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            WriteAuthCookie(authResult.AccessToken, authResult.ExpiresAtUtc);
            return Ok(authResult);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email, UserName, and Password are required.");
            }

            try
            {
                var authResult = await _authService.RegisterAsync(request);
                WriteAuthCookie(authResult.AccessToken, authResult.ExpiresAtUtc);
                return Ok(authResult);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(AuthCookieName);
            return Ok(new { message = "Logged out successfully." });
        }

        private void WriteAuthCookie(string token, DateTime expiresAtUtc)
        {
            Response.Cookies.Append(AuthCookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expiresAtUtc
            });
        }

    }
}
