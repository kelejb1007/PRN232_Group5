using Intelligence_Book_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Intelligence_Book_WEB.Controllers.User
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> OrderDetail(int orderId)
        {
            using var client = new HttpClient();

            try
            {
                if (orderId <= 0)
                {
                    ViewBag.Error = "Mã đơn hàng không hợp lệ";
                    return View(new OrderDetailViewModel());
                }

                // 1. gọi API đánh dấu đã thanh toán
                var paidResponse = await client.PostAsync(
                    $"https://localhost:7287/api/cart/mark-paid/{orderId}",
                    null
                );

                // nếu fail thì vẫn cho xem chi tiết, không chặn cứng
                if (!paidResponse.IsSuccessStatusCode)
                {
                    var paidError = await paidResponse.Content.ReadAsStringAsync();
                    ViewBag.Warning = $"Không cập nhật được trạng thái thanh toán: {paidError}";
                }

                // 2. lấy lại chi tiết đơn hàng
                var response = await client.GetAsync($"https://localhost:7287/api/cart/order-detail/{orderId}");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Không tải được chi tiết đơn hàng";
                    return View(new OrderDetailViewModel());
                }

                var json = await response.Content.ReadAsStringAsync();

                var order = JsonSerializer.Deserialize<OrderDetailViewModel>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (order == null)
                {
                    ViewBag.Error = "Không đọc được dữ liệu đơn hàng";
                    return View(new OrderDetailViewModel());
                }

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