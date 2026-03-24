using Intelligence_Book_WEB.Models.Dto;

namespace Intelligence_Book_WEB.Mapper
{
    public static class CouponMapper
    {
        public static CouponVm ToVm(this CouponDto d)
        {
            return new CouponVm
            {
                CouponId = d.CouponId,
                Code = d.Code,
                DiscountPercent = d.DiscountPercent,
                ExpiryDate = d.ExpiryDate,
                quantity = d.quantity
            };
        }
    }
}