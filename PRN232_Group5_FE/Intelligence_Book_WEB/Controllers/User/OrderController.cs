using Intelligence_Book_WEB.Models;
using Intelligence_Book_WEB.Services.User.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Intelligence_Book_WEB.Controllers.User
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProfileService _profileService;
        private const string AccessTokenCookie = "access_token";
        private readonly IHttpClientFactory _factory;

        public OrderController(IOrderService orderService, IProfileService profileService, IHttpClientFactory factory)
        {
            _orderService = orderService;
            _profileService = profileService;
            _factory = factory;
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
        public async Task<IActionResult> OrderDetail(int orderId)
        {
            try
            {
                if (orderId <= 0)
                {
                    ViewBag.Error = "Mã đơn hàng không hợp lệ";
                    return View(new OrderDetailViewModel());
                }

                var client = _factory.CreateClient("MyAPI");

                var response = await client.GetAsync(
                    $"api/cart/order-detail/{orderId}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = $"API lỗi: {error}";
                    return View(new OrderDetailViewModel());
                }

                var json = await response.Content.ReadAsStringAsync();

                var order = JsonSerializer.Deserialize<OrderDetailViewModel>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }
                );

                return View(order);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new OrderDetailViewModel());
            }
        }
    }
}
