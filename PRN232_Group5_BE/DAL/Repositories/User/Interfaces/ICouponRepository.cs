using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.Repositories.User.Interfaces
{
    public interface ICouponRepository
    {
        Task<Coupon?> GetByCode(string code);
        Task Update(Coupon coupon);
        Task<List<Coupon>> GetAllValidCoupons();
    }
}
