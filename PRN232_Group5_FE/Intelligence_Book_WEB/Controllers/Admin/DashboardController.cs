using Intelligence_Book_WEB.Services.Admin.Interfaces;
using Intelligence_Book_WEB.Services.User.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Intelligence_Book_WEB.Controllers.Admin
{
    public class DashboardController : BaseAdminController
    {
        private readonly IDashboardService _dashboardService;
        private const string AccessTokenCookie = "access_token";

        public DashboardController(IDashboardService dashboardService, IProfileService profileService)
            : base(profileService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = Request.Cookies[AccessTokenCookie];
            // Session and Role checks are handled by BaseAdminController

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
