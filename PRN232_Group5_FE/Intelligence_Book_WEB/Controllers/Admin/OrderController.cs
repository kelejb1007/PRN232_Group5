using Intelligence_Book_WEB.Mapper;
using Intelligence_Book_WEB.Models;
using Intelligence_Book_WEB.Models.Dto;
using Intelligence_Book_WEB.Models.Enums;
using Intelligence_Book_WEB.Services.User.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Intelligence_Book_WEB.Controllers.Admin
{
    [Route("Admin/Order")]
    public class OrderController : BaseAdminController
    {
        private readonly IHttpClientFactory _http;

        public OrderController(IHttpClientFactory http, IProfileService profileService) 
            : base(profileService)
        {
            _http = http;
        }

        private JsonSerializerOptions GetJsonOptions()
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }

        [HttpGet("OrderList")]
        public async Task<IActionResult> OrderList(string? search, OrderStatus? status, int page = 1, int pageSize = 5)
        {
            var client = _http.CreateClient("MyAPI");

            var qs = new List<string> { $"page={page}", $"pageSize={pageSize}" };
            if (!string.IsNullOrWhiteSpace(search)) qs.Add($"search={Uri.EscapeDataString(search)}");
            if (status.HasValue) qs.Add($"status={status.Value}");

            var url = $"api/admin/orders?{string.Join("&", qs)}";

            try 
            {
                var dto = await client.GetFromJsonAsync<PagedResultDto<OrderDto>>(url, GetJsonOptions()) 
                          ?? new PagedResultDto<OrderDto>();

                var vm = new PagedResultVm<OrderVm>
                {
                    Page = dto.Page,
                    PageSize = dto.PageSize,
                    TotalItems = dto.TotalItems,
                    TotalPages = dto.TotalPages,
                    Items = (dto.Items ?? new List<OrderDto>()).Select(x => x.ToVm()).ToList()
                };

                ViewBag.Search = search;
                ViewBag.Status = status;
                return View("~/Views/Admin/Order/OrderList.cshtml", vm);
            }
            catch(Exception ex)
            {
                TempData["Message"] = "Lỗi kết nối API: " + ex.Message;
                return View("~/Views/Admin/Order/OrderList.cshtml", new PagedResultVm<OrderVm>());
            }
        }

        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var client = _http.CreateClient("MyAPI");
            try
            {
                var dto = await client.GetFromJsonAsync<OrderDetailDto>($"api/admin/orders/{id}", GetJsonOptions());
                if (dto == null)
                {
                    TempData["Message"] = "Không tìm thấy đơn hàng.";
                    return RedirectToAction(nameof(OrderList));
                }
                var vm = dto.ToDetailVm();
                return View("~/Views/Admin/Order/OrderDetail.cshtml", vm);
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Lỗi kết nối API: " + ex.Message;
                return RedirectToAction(nameof(OrderList));
            }
        }

        [HttpPost("UpdateStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus newStatus)
        {
            var client = _http.CreateClient("MyAPI");
            var payload = new { newStatus = newStatus };
            
            var res = await client.PutAsJsonAsync($"api/admin/orders/{id}/status", payload, GetJsonOptions());
            if (!res.IsSuccessStatusCode)
            {
                var err = await SafeReadApiMessage(res);
                TempData["Message"] = err ?? "Cập nhật thất bại!";
            }
            else
            {
                TempData["Message"] = "Cập nhật thành công!";
            }
            return RedirectToAction(nameof(OrderList));
        }

        [HttpPost("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var client = _http.CreateClient("MyAPI");
            var res = await client.PutAsync($"api/admin/orders/{id}/cancel", null);
            
            if (!res.IsSuccessStatusCode)
            {
                var err = await SafeReadApiMessage(res);
                TempData["Message"] = "Hủy thất bại: " + err;
            }
            else
            {
                TempData["Message"] = "Hủy đơn hàng thành công!";
            }
            return RedirectToAction(nameof(OrderList));
        }

        private async Task<string?> SafeReadApiMessage(HttpResponseMessage res)
        {
            try
            {
                var msg = await res.Content.ReadFromJsonAsync<ApiMessage>(GetJsonOptions());
                if (!string.IsNullOrWhiteSpace(msg?.message)) return msg!.message;
                if (!string.IsNullOrWhiteSpace(msg?.Message)) return msg!.Message;
            }
            catch { /* ignore */ }
            return null;
        }

        private class ApiMessage
        {
            public string? message { get; set; }
            public string? Message { get; set; }
        }
    }
}
