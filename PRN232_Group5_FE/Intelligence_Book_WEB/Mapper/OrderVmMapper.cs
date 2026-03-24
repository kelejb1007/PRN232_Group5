using Intelligence_Book_WEB.Models.Dto;

namespace Intelligence_Book_WEB.Mapper
{
    public static class OrderVmMapper
    {
        public static OrderVm ToVm(this OrderDto dto)
        {
            if (dto == null) return null!;
            return new OrderVm
            {
                OrderId = dto.OrderId,
                UserId = dto.UserId,
                CustomerName = dto.CustomerName ?? "Unknown",
                OrderDate = dto.OrderDate,
                TotalAmount = dto.TotalAmount,
                Status = dto.Status,
                ShippingAddress = dto.ShippingAddress,
                CouponId = dto.CouponId
            };
        }
    }
}
