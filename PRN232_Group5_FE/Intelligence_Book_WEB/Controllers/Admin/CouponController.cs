using Intelligence_Book_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Intelligence_Book_WEB.Controllers.Admin
{
    [Route("Admin/Coupon")]
    public class CouponController : Controller
    {
        private readonly IHttpClientFactory _http;

        public CouponController(IHttpClientFactory http)
        {
            _http = http;
        }

        // LIST: /Admin/Coupon/CouponList?search=...&page=1
        [HttpGet("CouponList")]
        public async Task<IActionResult> CouponList(string? search, int page = 1, int pageSize = 5)
        {
            var client = _http.CreateClient("Api");

            var qs = new List<string> { $"page={page}", $"pageSize={pageSize}" };
            if (!string.IsNullOrWhiteSpace(search))
                qs.Add($"search={Uri.EscapeDataString(search)}");

            var url = $"api/admin/coupons?{string.Join("&", qs)}";

            var data = await client.GetFromJsonAsync<PagedResultVm<CouponVm>>(url)
                       ?? new PagedResultVm<CouponVm>();

            ViewBag.Search = search;

            return View("~/Views/Admin/Coupon/CouponList.cshtml", data);
        }

        // DETAILS: /Admin/Coupon/CouponDetails?id=1
        [HttpGet("CouponDetails")]
        public async Task<IActionResult> CouponDetails(int id)
        {
            var client = _http.CreateClient("Api");
            var coupon = await client.GetFromJsonAsync<CouponVm>($"api/admin/coupons/{id}");

            if (coupon == null)
            {
                TempData["Message"] = "Coupon not found!";
                return RedirectToAction(nameof(CouponList));
            }

            return View("~/Views/Admin/Coupon/CouponDetails.cshtml", coupon);
        }

        // FORM ADD
        [HttpGet("CouponCreate")]
        public IActionResult CouponCreate()
        {
            return View("~/Views/Admin/Coupon/CouponForm.cshtml", new CouponVm());
        }

        // FORM UPDATE
        [HttpGet("CouponUpdate")]
        public async Task<IActionResult> CouponUpdate(int id)
        {
            var client = _http.CreateClient("Api");
            var coupon = await client.GetFromJsonAsync<CouponVm>($"api/admin/coupons/{id}");

            if (coupon == null)
            {
                TempData["Message"] = "Coupon not found!";
                return RedirectToAction(nameof(CouponList));
            }

            return View("~/Views/Admin/Coupon/CouponForm.cshtml", coupon);
        }

        // POST SAVE (Create / Update)
        [HttpPost("CouponSave")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CouponSave(CouponVm model)
        {
            var client = _http.CreateClient("Api");

            model.Code = (model.Code ?? "").Trim();

            // ✅ Body đúng model BE mới: có quantity, không IsActive
            var payload = new
            {
                couponId = model.CouponId,
                code = model.Code,
                discountPercent = model.DiscountPercent,
                expiryDate = model.ExpiryDate,
                quantity = model.quantity
            };

            HttpResponseMessage res;

            if (model.CouponId == 0)
            {
                res = await client.PostAsJsonAsync("api/admin/coupons", payload);
            }
            else
            {
                res = await client.PutAsJsonAsync($"api/admin/coupons/{model.CouponId}", payload);
            }

            if (!res.IsSuccessStatusCode)
            {
                var err = await res.Content.ReadFromJsonAsync<ApiMessageVm>();
                ViewBag.Error = err?.Message ?? "Save failed!";
                return View("~/Views/Admin/Coupon/CouponForm.cshtml", model);
            }

            TempData["Message"] = model.CouponId == 0 ? "Created successfully!" : "Updated successfully!";
            return RedirectToAction(nameof(CouponList));
        }

        // DELETE
        [HttpPost("CouponDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CouponDelete(int id)
        {
            var client = _http.CreateClient("Api");
            var res = await client.DeleteAsync($"api/admin/coupons/{id}");

            if (!res.IsSuccessStatusCode)
            {
                TempData["Message"] = "Delete failed!";
                return RedirectToAction(nameof(CouponDetails), new { id });
            }

            TempData["Message"] = "Deleted successfully!";
            return RedirectToAction(nameof(CouponList));
        }
    }

    // helper đọc message từ API
    public class ApiMessageVm
    {
        public string? Message { get; set; }
    }
}