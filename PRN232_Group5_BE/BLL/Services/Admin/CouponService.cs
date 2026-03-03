using BLL.Services.Admin.Interfaces;
using DAL.Models;
using DAL.Repositories.Admin.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Admin
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _repo;

        public CouponService(ICouponRepository repo)
        {
            _repo = repo;
        }

        public Task<(List<Coupon>, int)> GetPagedAsync(string? search, int page, int pageSize)
            => _repo.GetPagedAsync(search, page, pageSize);

        public Task<Coupon?> GetByIdAsync(int id)
            => _repo.GetByIdAsync(id);

        public Task<Coupon> CreateAsync(Coupon coupon)
            => _repo.CreateAsync(coupon);

        public Task<bool> UpdateAsync(Coupon coupon)
            => _repo.UpdateAsync(coupon);

        public Task<bool> DeleteAsync(int id)
            => _repo.DeleteAsync(id);
    }
}
