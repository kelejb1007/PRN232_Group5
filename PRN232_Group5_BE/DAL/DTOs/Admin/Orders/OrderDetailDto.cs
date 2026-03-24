using DAL.Models.Enums;
using System;
using System.Collections.Generic;

namespace DAL.DTOs.Admin.Orders
{
    public class OrderDetailDto : OrderDto
    {
        public string? CouponCode { get; set; }
        public double? CouponDiscountPercentage { get; set; }

        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
}
