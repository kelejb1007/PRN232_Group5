using DAL.DTOs.Admin.Coupons;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Admin.Interfaces
{
    public interface ICouponService
    {
        Task<(List<CouponDto> items, int totalItems)> GetPagedAsync(string? search, int page, int pageSize);
        Task<CouponDto?> GetByIdAsync(int id);

        Task<CouponDto> CreateAsync(CouponCreateDto dto);
        Task<bool> UpdateAsync(int id, CouponUpdateDto dto);  // chỉ expiry + quantity
        Task<bool> SoftDeleteAsync(int id);
    }
}
