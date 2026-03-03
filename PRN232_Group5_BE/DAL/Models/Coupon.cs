using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Coupon
    {
        [Key]
        public int CouponId { get; set; }
        [Required, StringLength(50)]
        public string Code { get; set; } = string.Empty;
        public int DiscountPercent { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
