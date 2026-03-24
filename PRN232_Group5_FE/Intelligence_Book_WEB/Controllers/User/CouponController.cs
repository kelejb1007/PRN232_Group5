//using Microsoft.AspNetCore.Mvc;

//namespace Intelligence_Book_WEB.Controllers.User
//{

//    [Route("api")]
//    [ApiController]
//    public class CouponController : ControllerBase
//    {
//        // GET api/coupon/apply?code=XXX&total=YYY
//        [HttpGet("coupon/apply")]
//        public IActionResult ApplyCoupon(string code, decimal total)
//        {
//            if (string.IsNullOrEmpty(code))
//                return BadRequest("Vui lòng nhập mã coupon");

//            decimal discount = 0;

//            // Ví dụ logic: giảm 2000 nếu tổng >=1000
//            if (code == "DISCOUNT2K" && total >= 1000)
//                discount = 2000;
//            else
//                return BadRequest("Coupon không hợp lệ hoặc không đủ điều kiện");

//            return Ok(new { discountAmount = discount });
//        }
//    }
//}
