
using Intelligence_Book_WEB.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Models.UserAccount.AuthDTO;

namespace Intelligence_Book_WEB.Controllers.Admin
{
    public class AuthController : Controller
    {
        private const string AccessTokenCookie = "access_token";
        private readonly IAuthService _authService;

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
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            {
                ViewBag.Error = "UserName and Password are required.";
                return View(request);
            }

            var result = await _authService.LoginAsync(request);
            if (result == null)
            {
                ViewBag.Error = "Invalid username or password.";
                return View(request);
            }

            WriteAccessTokenCookie(result.AccessToken, result.ExpiresAtUtc);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                ViewBag.Error = "Name, UserName and Password are required.";
                return View(request);
            }

            var result = await _authService.RegisterAsync(request);
            if (result == null)
            {
                ViewBag.Error = "Register failed. Username may already exist.";
                return View(request);
            }

            WriteAccessTokenCookie(result.AccessToken, result.ExpiresAtUtc);
            return RedirectToAction("Login", "Auth");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Cookies[AccessTokenCookie];
            await _authService.LogoutAsync(token);

            Response.Cookies.Delete(AccessTokenCookie);
            return RedirectToAction("Index", "Home");
        }



        private void WriteAccessTokenCookie(string token, DateTime expiresAtUtc)
        {
            Response.Cookies.Append(AccessTokenCookie, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expiresAtUtc
            });
        }

    }
}
