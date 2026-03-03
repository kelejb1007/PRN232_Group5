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
        Task<(List<Coupon>, int)> GetPagedAsync(string? search, int page, int pageSize);
        Task<Coupon?> GetByIdAsync(int id);
        Task<Coupon> CreateAsync(Coupon coupon);
        Task<bool> UpdateAsync(Coupon coupon);
        Task<bool> DeleteAsync(int id);
    }
}
