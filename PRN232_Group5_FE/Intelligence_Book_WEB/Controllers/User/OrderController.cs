using Intelligence_Book_WEB.Services.User.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Intelligence_Book_WEB.Controllers.User
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProfileService _profileService;
        private const string AccessTokenCookie = "access_token";

        public OrderController(IOrderService orderService, IProfileService profileService)
        {
            _orderService = orderService;
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var token = Request.Cookies[AccessTokenCookie];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var profile = await _profileService.GetProfileAsync(token);
            ViewBag.ProfileInfo = profile;
            ViewBag.ActiveTab = "orders";
            
            var orders = await _orderService.GetOrderHistoryAsync(token);
            return View("~/Views/User/Order/History.cshtml", orders);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var token = Request.Cookies[AccessTokenCookie];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var order = await _orderService.GetOrderDetailsAsync(token, id);
            if (order == null) return NotFound();

            return View("~/Views/User/Order/Details.cshtml", order);
        }
    }
}
