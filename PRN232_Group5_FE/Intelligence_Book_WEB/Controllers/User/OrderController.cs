using Intelligence_Book_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Intelligence_Book_WEB.Controllers.User
{
    public class OrderController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public OrderController(IHttpClientFactory factory)
        {
            _factory = factory;
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