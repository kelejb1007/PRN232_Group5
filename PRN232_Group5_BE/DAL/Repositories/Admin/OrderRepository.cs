using DAL.Data;
using DAL.Models;
using DAL.Models.Enums;
using DAL.Repositories.Admin.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Admin
{
    public class OrderRepository : IOrderRepository
    {
        private readonly Intelligence_Book_APIContext _db;

        public OrderRepository(Intelligence_Book_APIContext db)
        {
            _db = db;
        }

        public async Task<(List<Order> items, int totalItems)> GetPagedAsync(string? search, OrderStatus? status, int page, int pageSize)
        {
            var q = _db.Orders
                .Include(o => o.UserAccount)
                .AsNoTracking().AsQueryable();

            if (status.HasValue)
            {
                q = q.Where(x => x.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(x =>
                    x.OrderId.ToString().Contains(s) ||
                    (x.UserAccount != null && x.UserAccount.FullName != null && x.UserAccount.FullName.ToLower().Contains(s))
                );
            }

            var total = await q.CountAsync();

            var items = await q
                .OrderByDescending(x => x.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public Task<Order?> GetByIdAsync(int id)
            => _db.Orders
                .Include(o => o.UserAccount)
                .Include(o => o.Coupon)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Book)
                .FirstOrDefaultAsync(x => x.OrderId == id);

        public async Task<bool> UpdateAsync(Order order)
        {
            _db.Orders.Update(order);
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
