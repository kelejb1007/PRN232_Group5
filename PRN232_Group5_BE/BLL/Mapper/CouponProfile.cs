using AutoMapper;
using DAL.DTOs.Admin.Coupons;
using DAL.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BLL.Mapper
{
    public class CouponProfile : Profile
    {
        public CouponProfile()
        {
            // Entity -> DTO (GET list / GET detail)
            CreateMap<Coupon, CouponDto>();

            // Create DTO -> Entity (POST)
            CreateMap<CouponCreateDto, Coupon>()
                .ForMember(d => d.CouponId, opt => opt.Ignore());

            // Update DTO -> Entity (PUT) - chỉ update ExpiryDate + quantity
            CreateMap<CouponUpdateDto, Coupon>()
                .ForMember(d => d.CouponId, opt => opt.Ignore())
                .ForMember(d => d.Code, opt => opt.Ignore())
                .ForMember(d => d.DiscountPercent, opt => opt.Ignore());
        }
    }
}