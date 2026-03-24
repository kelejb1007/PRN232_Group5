using Intelligence_Book_WEB.Models.Enums;
using System;

namespace Intelligence_Book_WEB.Models.Dto
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string? ShippingAddress { get; set; }
        public int? CouponId { get; set; }
    }
}
