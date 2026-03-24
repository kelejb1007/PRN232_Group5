using Intelligence_Book_WEB.Services.Admin.Interfaces;
using Intelligence_Book_WEB.Services.User.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Intelligence_Book_WEB.Controllers.Admin
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IProfileService _profileService;
        private const string AccessTokenCookie = "access_token";

        public DashboardController(IDashboardService dashboardService, IProfileService profileService)
        {
            _dashboardService = dashboardService;
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = Request.Cookies[AccessTokenCookie];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var profile = await _profileService.GetProfileAsync(token);
            if (profile == null || profile.Role != "Admin") return RedirectToAction("Index", "Home");

            ViewBag.ProfileInfo = profile;

            try
            {
                var summary = await _dashboardService.GetDashboardSummaryAsync(token);
                return View("~/Views/Admin/Dashboard/Index.cshtml", summary);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Dashboard Error: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
