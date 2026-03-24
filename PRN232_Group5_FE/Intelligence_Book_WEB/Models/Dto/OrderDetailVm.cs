using Intelligence_Book_WEB.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Intelligence_Book_WEB.Models.Dto
{
    public class OrderDetailVm : OrderVm
    {
        public string? CouponCode { get; set; }
        public double? CouponDiscountPercentage { get; set; }

        public List<OrderItemVm> OrderItems { get; set; } = new List<OrderItemVm>();
    }
}
