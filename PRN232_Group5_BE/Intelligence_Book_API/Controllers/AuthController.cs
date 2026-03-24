using BLL.Services.Interfaces;
using DAL.DTOs.UserAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DAL.DTOs.UserAccount.AuthDTO;

namespace Intelligence_Book_API.Controllers
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
                return BadRequest("Tên đăng nhập và Mật khẩu là bắt buộc.");
            }

            var authResult = await _authService.LoginAsync(request);
            if (authResult == null)
            {
                return Unauthorized("Tên đăng nhập hoặc mật khẩu không đúng.");
            }

            WriteAuthCookie(authResult.AccessToken, authResult.ExpiresAtUtc);
            return Ok(authResult);
        }

        [HttpPost("google-login")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDTO request)
        {
            try
            {
                var authResult = await _authService.GoogleLoginAsync(request);
                WriteAuthCookie(authResult.AccessToken, authResult.ExpiresAtUtc);
                return Ok(authResult);
            }
            catch (Exception ex)
            {
                return BadRequest("Mã Google không hợp lệ: " + ex.Message);
            }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email, Tên đăng nhập và Mật khẩu là bắt buộc.");
            }

            try
            {
                var success = await _authService.RegisterAsync(request);
                return Ok(new { success });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("verify-email")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDTO request)
        {
            var success = await _authService.VerifyEmailAsync(request);
            if (success)
            {
                return Ok(new { message = "Xác thực email thành công" });
            }
            return BadRequest("Mã xác thực không hợp lệ hoặc đã hết hạn.");
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO request)
        {
            try
            {
                var success = await _authService.ForgotPasswordAsync(request);
                return Ok(new { message = "Nếu email của bạn đã được đăng ký, bạn sẽ nhận được mã OTP sớm." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO request)
        {
            var success = await _authService.ResetPasswordAsync(request);
            if (success)
            {
                return Ok(new { message = "Mật khẩu đã được đặt lại thành công." });
            }
            return BadRequest("Mã xác thực không hợp lệ hoặc đã hết hạn.");
        }

        [HttpPost("resend-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendOtp([FromQuery] string email, [FromQuery] string type)
        {
            var success = await _authService.ResendOtpAsync(email, type);
            if (success)
            {
                return Ok(new { message = "Mã OTP đã được gửi lại thành công." });
            }
            return BadRequest("Không thể gửi lại mã OTP. Vui lòng thử lại sau.");
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(AuthCookieName);
            return Ok(new { message = "Đăng xuất thành công." });
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
