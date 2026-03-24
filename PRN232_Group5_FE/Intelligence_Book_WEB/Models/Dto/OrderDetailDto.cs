using Intelligence_Book_WEB.Models.Enums;
using System.Collections.Generic;
using System.Linq;
using Intelligence_Book_WEB.Mapper;

namespace Intelligence_Book_WEB.Models.Dto
{
    public class OrderDetailDto : OrderDto
    {
        public string? CouponCode { get; set; }
        public double? CouponDiscountPercentage { get; set; }

        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();

        public OrderDetailVm ToDetailVm()
        {
            var baseVm = this.ToVm();
            return new OrderDetailVm
            {
                OrderId = baseVm.OrderId,
                UserId = baseVm.UserId,
                CustomerName = baseVm.CustomerName,
                OrderDate = baseVm.OrderDate,
                TotalAmount = baseVm.TotalAmount,
                Status = baseVm.Status,
                ShippingAddress = baseVm.ShippingAddress,
                CouponId = baseVm.CouponId,
                CouponCode = this.CouponCode,
                CouponDiscountPercentage = this.CouponDiscountPercentage,
                OrderItems = this.OrderItems?.Select(i => new OrderItemVm 
                {
                    OrderItemId = i.OrderItemId,
                    OrderId = i.OrderId,
                    BookId = i.BookId,
                    BookName = i.BookName,
                    Quantity = i.Quantity,
                    PriceAtPurchase = i.PriceAtPurchase
                }).ToList() ?? new List<OrderItemVm>()
            };
        }
    }
}
