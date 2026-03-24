using DAL.Data;
using DAL.Models;
using DAL.Repositories.Admin.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Admin
{
    public class CouponRepository : ICouponRepository
    {
        private readonly Intelligence_Book_APIContext _db;

        public CouponRepository(Intelligence_Book_APIContext db)
        {
            _db = db;
        }

        public async Task<(List<Coupon> items, int totalItems)> GetPagedAsync(string? search, int page, int pageSize)
        {
            // ✅ Ẩn soft delete: quantity == 0
            var q = _db.Coupons.AsNoTracking().Where(x => x.quantity > 0);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(x =>
                    x.Code.Contains(s) ||
                    x.CouponId.ToString().Contains(s) ||
                    x.DiscountPercent.ToString().Contains(s) ||
                    x.quantity.ToString().Contains(s)
                );
            }

            var total = await q.CountAsync();

            var today = DateTime.Today;
            var items = await q
                .OrderByDescending(x => !x.ExpiryDate.HasValue || x.ExpiryDate.Value.Date >= today)
                .ThenByDescending(x => x.CouponId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public Task<Coupon?> GetByIdAsync(int id)
            => _db.Coupons.FirstOrDefaultAsync(x => x.CouponId == id);

        public Task<bool> CodeExistsAsync(string code, int? ignoreId = null)
        {
            code = (code ?? "").Trim();
            return _db.Coupons.AnyAsync(x => x.Code == code && (!ignoreId.HasValue || x.CouponId != ignoreId.Value));
        }

        public async Task<Coupon> AddAsync(Coupon coupon)
        {
            _db.Coupons.Add(coupon);
            await _db.SaveChangesAsync();
            return coupon;
        }

        public async Task<bool> UpdateAsync(Coupon coupon)
        {
            _db.Coupons.Update(coupon);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> SaveChangesAsync()
            => await _db.SaveChangesAsync() > 0;
    }
}
