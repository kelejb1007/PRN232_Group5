using Intelligence_Book_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Intelligence_Book_WEB.Controllers.User
{
    public class OrderController : Controller
    {
        private readonly HttpClient _client;

        public OrderController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient();
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> OrderDetail(int orderId)
        {
            try
            {
                if (orderId <= 0)
                {
                    ViewBag.Error = "Mã đơn hàng không hợp lệ";
                    return View(new OrderDetailViewModel());
                }

                // ❌ KHÔNG gọi mark-paid ở đây nữa
                // 👉 nên gọi ở callback thanh toán hoặc backend

                // ✅ gọi API lấy chi tiết đơn hàng
                var response = await _client.GetAsync(
                    $"https://localhost:7287/api/order/{orderId}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = $"API lỗi: {error}";
                    return View(new OrderDetailViewModel());
                }

                var json = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(json))
                {
                    ViewBag.Error = "API trả về dữ liệu rỗng";
                    return View(new OrderDetailViewModel());
                }

                var order = JsonSerializer.Deserialize<OrderDetailViewModel>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }
                );

                if (order == null)
                {
                    ViewBag.Error = "Không đọc được dữ liệu đơn hàng";
                    return View(new OrderDetailViewModel());
                }

                return View(order);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi hệ thống: {ex.Message}";
                return View(new OrderDetailViewModel());
            }
        }
    }
}