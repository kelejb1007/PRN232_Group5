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
        private readonly Intelligence_Book_APIContext _context;

        public CouponRepository(Intelligence_Book_APIContext context)
        {
            _context = context;
        }

        public async Task<(List<Coupon>, int)> GetPagedAsync(string? search, int page, int pageSize)
        {
            var query = _context.Coupons.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    c.Code.Contains(search) ||
                    c.CouponId.ToString().Contains(search) ||
                    c.DiscountPercent.ToString().Contains(search));
            }

            int totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.CouponId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }

        public async Task<Coupon?> GetByIdAsync(int id)
        {
            return await _context.Coupons.FindAsync(id);
        }

        public async Task<Coupon> CreateAsync(Coupon coupon)
        {
            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();
            return coupon;
        }

        public async Task<bool> UpdateAsync(Coupon coupon)
        {
            var existing = await _context.Coupons.FindAsync(coupon.CouponId);
            if (existing == null) return false;

            existing.Code = coupon.Code;
            existing.DiscountPercent = coupon.DiscountPercent;
            existing.ExpiryDate = coupon.ExpiryDate;
            existing.quantity = coupon.quantity;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return false;

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
