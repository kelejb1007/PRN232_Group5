using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data;
using DAL.Models;
using DAL.Models.Enums;
using DAL.Repositories.User.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.User
{
    public class OrderRepository : IOrderRepository
    {
        private readonly Intelligence_Book_APIContext _context;

        public OrderRepository(Intelligence_Book_APIContext context)
        {
            _context = context;
        }

        public async Task<bool> HasUserPurchasedBookAsync(int userId, int bookId)
        {
            // Kiểm tra xem User có đơn hàng nào đã được giao (Delivered) và chứa cuốn sách này không
            return await _context.Orders
                .Where(o => o.UserId == userId && o.Status == OrderStatus.Delivered)
                .AnyAsync(o => o.OrderItems.Any(oi => oi.BookId == bookId));
        }
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderDetailsAsync(int orderId, int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);
        }
    }
}
