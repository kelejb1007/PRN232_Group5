using BLL.Services.Admin.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace Intelligence_Book_API.Controllers.Admin
{
    [Route("api/admin/coupons")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _service;

        public CouponsController(ICouponService service)
        {
            _service = service;
        }

        // ========================= LIST =========================
        [HttpGet]
        public async Task<IActionResult> GetPaged(string? search, int page = 1, int pageSize = 5)
        {
            var (items, totalItems) = await _service.GetPagedAsync(search, page, pageSize);

            return Ok(new
            {
                page,
                pageSize,
                totalItems,
                totalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                items
            });
        }

        // ========================= GET BY ID =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var coupon = await _service.GetByIdAsync(id);
            if (coupon == null) return NotFound();

            return Ok(coupon);
        }

        // ========================= CREATE =========================
        [HttpPost]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            // 🔥 Validate thực tế

            if (coupon.DiscountPercent < 1 || coupon.DiscountPercent > 50)
                return BadRequest(new { message = "Discount must be between 1 and 50." });

            if (coupon.quantity < 0 || coupon.quantity > 1000)
                return BadRequest(new { message = "Quantity must be between 0 and 1000." });

            if (coupon.ExpiryDate.HasValue &&
                coupon.ExpiryDate.Value.Date < DateTime.Today)
                return BadRequest(new { message = "Expiry date cannot be in the past." });

            var created = await _service.CreateAsync(coupon);

            return Ok(created);
        }

        // ========================= UPDATE =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Coupon coupon)
        {
            if (id != coupon.CouponId)
                return BadRequest(new { message = "Invalid ID." });

            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            // ❌ Không cho update nếu expired
            var isExpired = existing.ExpiryDate.HasValue &&
                            existing.ExpiryDate.Value.Date < DateTime.Today;

            if (isExpired)
                return BadRequest(new { message = "Expired coupon cannot be updated." });

            // 🔥 Validate logic
            if (coupon.DiscountPercent != existing.DiscountPercent)
                return BadRequest(new { message = "Discount cannot be modified." });

            if (coupon.Code != existing.Code)
                return BadRequest(new { message = "Code cannot be modified." });

            if (coupon.quantity < 0 || coupon.quantity > 1000)
                return BadRequest(new { message = "Quantity must be between 0 and 1000." });

            if (coupon.ExpiryDate.HasValue &&
                coupon.ExpiryDate.Value.Date < DateTime.Today)
                return BadRequest(new { message = "Expiry date cannot be in the past." });

            var success = await _service.UpdateAsync(coupon);

            if (!success)
                return BadRequest(new { message = "Update failed." });

            return Ok(new { message = "Updated successfully." });
        }

        // ========================= SOFT DELETE =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            // ❌ Không cho delete nếu expired
            var isExpired = existing.ExpiryDate.HasValue &&
                            existing.ExpiryDate.Value.Date < DateTime.Today;

            if (isExpired)
                return BadRequest(new { message = "Expired coupon cannot be deleted." });

            // 🔥 Soft delete
            existing.quantity = 0;
            existing.ExpiryDate = DateTime.Today.AddDays(-1);

            var success = await _service.UpdateAsync(existing);

            if (!success)
                return BadRequest(new { message = "Delete failed." });

            return Ok(new { message = "Deleted successfully (soft delete)." });
        }
    }
}