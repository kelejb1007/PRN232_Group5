using Intelligence_Book_WEB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Models.UserAccount.AuthDTO;

namespace Intelligence_Book_WEB.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private const string AccessTokenCookie = "access_token";
        private const string RefreshTokenCookie = "refresh_token";

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO request)
        {
            if (!ModelState.IsValid) return View(request);

            var result = await _authService.LoginAsync(request);
            if (result == null)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View(request);
            }

            // Save tokens to Cookies
            WriteAccessTokenCookie(result.AccessToken, result.ExpiresAtUtc);
            
            // Role-based redirection
            if (result.Role == "Admin")
            {
                return RedirectToAction("Index", "Dashboard", new { area = "" });
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterRequestDTO request)
        {
            if (!ModelState.IsValid) return View(request);

            var success = await _authService.RegisterAsync(request);
            if (!success)
            {
                ViewBag.Error = "Đăng ký thất bại. Tên đăng nhập hoặc Email có thể đã tồn tại.";
                return View(request);
            }

            TempData["UserEmail"] = request.Email;
            TempData["ShowOtpPopup"] = true;
            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            var token = Request.Cookies[AccessTokenCookie];
            if (!string.IsNullOrEmpty(token))
            {
                await _authService.LogoutAsync(token);
            }

            Response.Cookies.Delete(AccessTokenCookie, new CookieOptions { 
                Secure = true, 
                SameSite = SameSiteMode.Lax 
            });
            Response.Cookies.Delete(RefreshTokenCookie);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDTO request)
        {
            if (!ModelState.IsValid) return View(request);
            var success = await _authService.ForgotPasswordAsync(request);
            if (success)
            {
                TempData["ForgotEmail"] = request.Email;
                TempData["ShowResetPopup"] = true;
            }
            return View(request);
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            return View(new ResetPasswordRequestDTO { Token = token, Email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                TempData["ForgotEmail"] = request.Email;
                TempData["ShowResetPopup"] = true;
                TempData["ResetError"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction("ForgotPassword");
            }

            var success = await _authService.ResetPasswordAsync(request);
            if (success)
            {
                TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công. Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }

            TempData["ForgotEmail"] = request.Email;
            TempData["ShowResetPopup"] = true;
            TempData["ResetError"] = "Đặt lại mật khẩu thất bại. Mã OTP có thể không đúng hoặc đã hết hạn.";
            return RedirectToAction("ForgotPassword");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                TempData["UserEmail"] = request.Email;
                TempData["ShowOtpPopup"] = true;
                TempData["OtpError"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction("Register");
            }

            var success = await _authService.VerifyEmailAsync(request);
            if (success)
            {
                TempData["SuccessMessage"] = "Email của bạn đã được xác thực. Bây giờ bạn có thể đăng nhập.";
                return RedirectToAction("Login");
            }

            TempData["UserEmail"] = request.Email;
            TempData["ShowOtpPopup"] = true;
            TempData["OtpError"] = "Mã xác thực không đúng hoặc đã hết hạn.";
            return RedirectToAction("Register");
        }

        [HttpPost]
        public async Task<IActionResult> ResendOtp(string email, string type)
        {
            if (string.IsNullOrEmpty(email)) return Json(new { success = false, message = "Email là bắt buộc." });
            
            var success = await _authService.ResendOtpAsync(email, type);
            if (success)
            {
                return Json(new { success = true, message = "Mã OTP đã được gửi lại thành công." });
            }
            return Json(new { success = false, message = "Không thể gửi lại mã OTP." });
        }

        [HttpPost]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.IdToken))
            {
                return BadRequest(new { success = false, message = "Thiếu mã Token" });
            }

            var result = await _authService.GoogleLoginAsync(request);
            if (result == null)
            {
                return BadRequest(new { success = false, message = "Đăng nhập Google thất bại" });
            }

            WriteAccessTokenCookie(result.AccessToken, result.ExpiresAtUtc);
            
            string redirectUrl = result.Role == "Admin" 
                ? Url.Action("Index", "Dashboard") 
                : Url.Action("Index", "Home");
                
            return Json(new { success = true, redirectUrl = redirectUrl });
        }

        private void WriteAccessTokenCookie(string token, DateTime expiresAtUtc)
        {
            Response.Cookies.Append(AccessTokenCookie, token, new CookieOptions
            {
                HttpOnly = false, // Allow navbar JS to read this!
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = expiresAtUtc,
                Path = "/"
            });
        }
    }
}
