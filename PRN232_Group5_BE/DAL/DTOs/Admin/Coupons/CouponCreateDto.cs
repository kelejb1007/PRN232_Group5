using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTOs.Admin.Coupons
{
    public class CouponCreateDto
    {
        public string Code { get; set; } = string.Empty;
        public int DiscountPercent { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int quantity { get; set; }
    }
}
