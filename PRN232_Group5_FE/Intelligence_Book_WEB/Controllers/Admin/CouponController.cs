using Intelligence_Book_WEB.Mapper;
using Intelligence_Book_WEB.Models;
using Intelligence_Book_WEB.Models.Dto;
using Intelligence_Book_WEB.Services.User.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Intelligence_Book_WEB.Controllers.Admin
{
    [Route("Admin/Coupon")]
    public class CouponController : BaseAdminController
    {
        private readonly IHttpClientFactory _http;

        public CouponController(IHttpClientFactory http, IProfileService profileService) 
            : base(profileService)
        {
            _http = http;
        }

        // GET: /Admin/Coupon/CouponList?search=...&page=1&pageSize=5
        [HttpGet("CouponList")]
        public async Task<IActionResult> CouponList(string? search, int page = 1, int pageSize = 5)
        {
            var client = _http.CreateClient("MyAPI");

            var qs = new List<string>
            {
                $"page={page}",
                $"pageSize={pageSize}"
            };
            if (!string.IsNullOrWhiteSpace(search))
                qs.Add($"search={Uri.EscapeDataString(search)}");

            var url = $"api/admin/coupons?{string.Join("&", qs)}";

            var dto = await client.GetFromJsonAsync<PagedResultDto<CouponDto>>(url)
                     ?? new PagedResultDto<CouponDto>();

            var vm = new PagedResultVm<CouponVm>
            {
                Page = dto.Page,
                PageSize = dto.PageSize,
                TotalItems = dto.TotalItems,
                TotalPages = dto.TotalPages,
                Items = (dto.Items ?? new List<CouponDto>()).Select(x => x.ToVm()).ToList()
            };

            ViewBag.Search = search;
            return View("~/Views/Admin/Coupon/CouponList.cshtml", vm);
        }

        // GET: /Admin/Coupon/CouponDetails?id=1
        [HttpGet("CouponDetails")]
        public async Task<IActionResult> CouponDetails(int id)
        {
            var client = _http.CreateClient("MyAPI");
            var dto = await client.GetFromJsonAsync<CouponDto>($"api/admin/coupons/{id}");

            if (dto == null)
            {
                TempData["Message"] = "Không tìm thấy Coupon!";
                return RedirectToAction(nameof(CouponList));
            }

            return View("~/Views/Admin/Coupon/CouponDetails.cshtml", dto.ToVm());
        }

        // GET: /Admin/Coupon/CouponCreate
        [HttpGet("CouponCreate")]
        public IActionResult CouponCreate()
        {
            // Create: được phép nhập code/discount/expiry/quantity (JS sẽ random code + validate)
            return View("~/Views/Admin/Coupon/CouponForm.cshtml", new CouponVm());
        }

        // GET: /Admin/Coupon/CouponUpdate?id=1
        [HttpGet("CouponUpdate")]
        public async Task<IActionResult> CouponUpdate(int id)
        {
            var client = _http.CreateClient("MyAPI");
            var dto = await client.GetFromJsonAsync<CouponDto>($"api/admin/coupons/{id}");

            if (dto == null)
            {
                TempData["Message"] = "Không tìm thấy Coupon!";
                return RedirectToAction(nameof(CouponList));
            }

            // FE lock nếu expired
            if (dto.IsExpired)
            {
                TempData["Message"] = "Không thể cập nhật mã giảm giá đã hết hạn.";
                return RedirectToAction(nameof(CouponDetails), new { id });
            }

            return View("~/Views/Admin/Coupon/CouponForm.cshtml", dto.ToVm());
        }

        // POST: /Admin/Coupon/CouponSave
        [HttpPost("CouponSave")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CouponSave(CouponVm model)
        {
            var client = _http.CreateClient("MyAPI");

            // Normalize
            model.Code = (model.Code ?? "").Trim();

            // ===================== CREATE =====================
            if (model.CouponId == 0)
            {
                // Create payload: đúng model BE (Code, DiscountPercent, ExpiryDate, quantity)
                var payload = new
                {
                    code = model.Code,
                    discountPercent = model.DiscountPercent,
                    expiryDate = model.ExpiryDate,
                    quantity = model.quantity
                };

                var res = await client.PostAsJsonAsync("api/admin/coupons", payload);

                if (!res.IsSuccessStatusCode)
                {
                    var err = await SafeReadApiMessage(res);
                    ViewBag.Error = err ?? "Create failed!";
                    return View("~/Views/Admin/Coupon/CouponForm.cshtml", model);
                }

                TempData["Message"] = "Tạo mã thành công!";
                return RedirectToAction(nameof(CouponList));
            }

            // ===================== UPDATE =====================
            // Update rule: chỉ cho sửa expiryDate + quantity
            var updatePayload = new
            {
                expiryDate = model.ExpiryDate,
                quantity = model.quantity
            };

            var up = await client.PutAsJsonAsync($"api/admin/coupons/{model.CouponId}", updatePayload);

            if (!up.IsSuccessStatusCode)
            {
                var err = await SafeReadApiMessage(up);
                ViewBag.Error = err ?? "Update failed!";
                return View("~/Views/Admin/Coupon/CouponForm.cshtml", model);
            }

            TempData["Message"] = "Cập nhật thành công!";
            return RedirectToAction(nameof(CouponDetails), new { id = model.CouponId });
        }

        // POST: /Admin/Coupon/CouponDelete
        [HttpPost("CouponDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CouponDelete(int id)
        {
            var client = _http.CreateClient("MyAPI");
            var res = await client.DeleteAsync($"api/admin/coupons/{id}");

            if (!res.IsSuccessStatusCode)
            {
                var err = await SafeReadApiMessage(res);
                TempData["Message"] = err ?? "Xóa thất bại!";
                return RedirectToAction(nameof(CouponDetails), new { id });
            }

            TempData["Message"] = "Xóa thành công!";
            return RedirectToAction(nameof(CouponList));
        }

        // ===================== Helpers =====================

        private async Task<string?> SafeReadApiMessage(HttpResponseMessage res)
        {
            try
            {
                var msg = await res.Content.ReadFromJsonAsync<ApiMessage>();
                if (!string.IsNullOrWhiteSpace(msg?.message)) return msg!.message;
                if (!string.IsNullOrWhiteSpace(msg?.Message)) return msg!.Message;
            }
            catch { /* ignore */ }
            return null;
        }

        private class ApiMessage
        {
            public string? message { get; set; } // some API use lower
            public string? Message { get; set; } // some API use upper
        }
    }
}