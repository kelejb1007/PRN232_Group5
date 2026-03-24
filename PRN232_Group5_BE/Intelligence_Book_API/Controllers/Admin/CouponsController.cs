using BLL.Services.Admin.Interfaces;
using DAL.DTOs.Admin.Common;
using DAL.DTOs.Admin.Coupons;
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

        [HttpGet]
        public async Task<IActionResult> GetPaged(string? search, int page = 1, int pageSize = 5)
        {
            var (items, totalItems) = await _service.GetPagedAsync(search, page, pageSize);

            var result = new PagedResultDto<CouponDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                Items = items
            };

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var coupon = await _service.GetByIdAsync(id);
            if (coupon == null) return NotFound(new { message = "Coupon not found." });
            return Ok(coupon);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CouponCreateDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return Ok(created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CouponUpdateDto dto)
        {
            try
            {
                var ok = await _service.UpdateAsync(id, dto);
                if (!ok) return BadRequest(new { message = "Update failed (not found or expired)." });
                return Ok(new { message = "Updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var ok = await _service.SoftDeleteAsync(id);
            if (!ok) return BadRequest(new { message = "Delete failed (not found or expired)." });
            return Ok(new { message = "Deleted successfully (soft delete)." });
        }
    }
}