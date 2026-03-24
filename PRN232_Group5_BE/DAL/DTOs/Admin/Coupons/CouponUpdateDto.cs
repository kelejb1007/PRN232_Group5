using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTOs.Admin.Coupons
{
    // Update chỉ cho sửa ExpiryDate + quantity
    public class CouponUpdateDto
    {
        public DateTime? ExpiryDate { get; set; }
        public int quantity { get; set; }
    }
}
