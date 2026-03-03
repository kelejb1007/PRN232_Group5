using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Admin.Interfaces
{
    public interface ICouponRepository
    {
        Task<(List<Coupon> items, int totalItems)> GetPagedAsync(string? search, int page, int pageSize);
        Task<Coupon?> GetByIdAsync(int id);

        Task<bool> CodeExistsAsync(string code, int? ignoreId = null);

        Task<Coupon> AddAsync(Coupon coupon);
        Task<bool> UpdateAsync(Coupon coupon);
        Task<bool> SaveChangesAsync();
    }
}
