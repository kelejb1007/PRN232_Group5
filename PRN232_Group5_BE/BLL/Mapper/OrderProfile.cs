using AutoMapper;
using DAL.DTOs.Admin.Orders;
using DAL.Models;

namespace BLL.Mapper
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.UserAccount != null ? src.UserAccount.FullName : "Unknown"));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.BookName, opt => opt.MapFrom(src => src.Book != null ? src.Book.Title : "Unknown"));

            CreateMap<Order, OrderDetailDto>()
                .IncludeBase<Order, OrderDto>()
                .ForMember(dest => dest.CouponCode, opt => opt.MapFrom(src => src.Coupon != null ? src.Coupon.Code : null))
                .ForMember(dest => dest.CouponDiscountPercentage, opt => opt.MapFrom(src => src.Coupon != null ? src.Coupon.DiscountPercent : (double?)null))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));
        }
    }
}
