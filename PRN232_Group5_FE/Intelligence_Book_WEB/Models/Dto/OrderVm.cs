using Intelligence_Book_WEB.Models.Enums;
using System;

namespace Intelligence_Book_WEB.Models.Dto
{
    public class OrderVm
    {
        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string? ShippingAddress { get; set; }
        public int? CouponId { get; set; }
        
        public string StatusText => Status.ToString();
        
        public string StatusColor => Status switch
        {
            OrderStatus.Pending => "neutral",
            OrderStatus.Processing => "processing",
            OrderStatus.Delivered => "ok",
            OrderStatus.Cancelled => "bad",
            _ => "neutral"
        };
    }
}
