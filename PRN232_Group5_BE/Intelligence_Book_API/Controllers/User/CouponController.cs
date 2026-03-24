using System;
using System.Linq;
using DAL.Data;
using Microsoft.AspNetCore.Mvc;
using DAL.Models;

namespace Intelligence_Book_API.Controllers.User
{
    [Route("api")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly Intelligence_Book_APIContext _context;
        public CouponController(Intelligence_Book_APIContext context)
        {
            _context = context;
        }

        // Áp dụng coupon
        [HttpGet("coupon/apply")]
        public IActionResult ApplyCoupon(string code, decimal total)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Vui lòng nhập mã coupon");

            // Lấy coupon hợp lệ
            var coupon = _context.Coupons
                .FirstOrDefault(c => c.Code == code
                                     && c.ExpiryDate >= DateTime.Now
                                     && c.quantity > 0);

            if (coupon == null)
                return BadRequest("Coupon không hợp lệ hoặc đã hết hạn");

            // Tính giảm giá theo phần trăm
            decimal discount = total * coupon.DiscountPercent / 100m;

            return Ok(new { discountAmount = discount });
        }

        // Lấy danh sách coupon còn hạn
        [HttpGet("coupon/list")]
        public IActionResult GetCouponList()
        {
            var coupons = _context.Coupons
                .Where(c => c.ExpiryDate >= DateTime.Now && c.quantity > 0)
                .Select(c => new
                {
                    c.Code,
                    c.DiscountPercent
                })
                .ToList();

            return Ok(coupons);
        }

        // Coupon hiện tại (chưa lưu gì ở DB user, trả default)
        [HttpGet("coupon/current")]
        public IActionResult GetCurrentCoupon()
        {
            return Ok(new { code = "", discountAmount = 0m });
        }
    }
}