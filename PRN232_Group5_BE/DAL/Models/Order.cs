using DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }

        // Sử dụng Enum cho Status
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public string? ShippingAddress { get; set; }
        public int? CouponId { get; set; }

        public UserAccount? UserAccount { get; set; }
        public Coupon? Coupon { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
