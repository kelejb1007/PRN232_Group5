using Intelligence_Book_WEB.Services.User.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.UserAccount;
using static Models.UserAccount.AuthDTO;

namespace Intelligence_Book_WEB.Controllers.User
{
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        private const string AccessTokenCookie = "access_token";

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = Request.Cookies[AccessTokenCookie];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var profile = await _profileService.GetProfileAsync(token);
            if (profile == null) return RedirectToAction("Login", "Auth");

            var updateDto = new UpdateProfileRequestDTO
            {
                FullName = profile.FullName,
                Phone = profile.Phone,
                Address = profile.Address
            };

            ViewBag.ProfileInfo = profile;
            return View("~/Views/User/Profile/Index.cshtml", updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateProfileRequestDTO request)
        {
            var token = Request.Cookies[AccessTokenCookie];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                var profile = await _profileService.GetProfileAsync(token);
                ViewBag.ProfileInfo = profile;
                return View("~/Views/User/Profile/Index.cshtml", request);
            }

            var success = await _profileService.UpdateProfileAsync(token, request);
            if (success)
            {
                TempData["SuccessMessage"] = "Profile updated successfully.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Failed to update profile.");
            var prof = await _profileService.GetProfileAsync(token);
            ViewBag.ProfileInfo = prof;
            return View("~/Views/User/Profile/Index.cshtml", request);
        }
    }
}
