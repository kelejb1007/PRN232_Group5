using System;
using System.Collections.Generic;
using DAL.Data;
using DAL.Models;
using DAL.Repositories.User.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.User
{

    public class CouponRepository : ICouponRepository
    {
        private readonly Intelligence_Book_APIContext _context;

        public CouponRepository(Intelligence_Book_APIContext context)
        {
            _context = context;
        }

        public async Task<Coupon?> GetByCode(string code)
        {
            return await _context.Coupons
                .FirstOrDefaultAsync(x => x.Code == code);
        }

        public async Task Update(Coupon coupon)
        {
            _context.Coupons.Update(coupon);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Coupon>> GetAllValidCoupons()
        {
            return await _context.Coupons.ToListAsync();
        }
    }
}
