using Intelligence_Book_WEB.Services.User.Interfaces;
using Intelligence_Book_WEB.Models.DeliveryAddress;
using Microsoft.AspNetCore.Mvc;

namespace Intelligence_Book_WEB.Controllers.User
{
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;
        private readonly IProfileService _profileService;
        private const string AccessTokenCookie = "access_token";

        public AddressController(IAddressService addressService, IProfileService profileService)
        {
            _addressService = addressService;
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = Request.Cookies[AccessTokenCookie];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var profile = await _profileService.GetProfileAsync(token);
            ViewBag.ProfileInfo = profile;
            ViewBag.ActiveTab = "addresses"; // Still keep for sidebar highlighting if needed

            var addresses = await _addressService.GetMyAddressesAsync(token);
            return View("~/Views/User/Address/Index.cshtml", addresses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeliveryAddressDTO request)
        {
            var token = Request.Cookies[AccessTokenCookie];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            if (ModelState.IsValid)
            {
                await _addressService.AddAddressAsync(token, request);
                TempData["SuccessMessage"] = "Address added successfully.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DeliveryAddressDTO request)
        {
            var token = Request.Cookies[AccessTokenCookie];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            if (ModelState.IsValid)
            {
                await _addressService.UpdateAddressAsync(token, id, request);
                TempData["SuccessMessage"] = "Address updated successfully.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var token = Request.Cookies[AccessTokenCookie];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            await _addressService.DeleteAddressAsync(token, id);
            TempData["SuccessMessage"] = "Address deleted successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefault(int id)
        {
            var token = Request.Cookies[AccessTokenCookie];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            await _addressService.SetDefaultAddressAsync(token, id);
            TempData["SuccessMessage"] = "Default address updated.";
            return RedirectToAction("Index");
        }
    }
}
