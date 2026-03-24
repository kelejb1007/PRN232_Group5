using DAL.Data;
using DAL.DTOs.Dashboard;
using DAL.Models.Enums;
using DAL.Repositories.Admin.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Admin
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly Intelligence_Book_APIContext _context;

        public DashboardRepository(Intelligence_Book_APIContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<int> GetTotalOrdersAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task<int> GetTotalCustomersAsync()
        {
            return await _context.UserAccounts.CountAsync(u => u.Role == UserRole.Customer);
        }

        public async Task<int> GetTotalBooksAsync()
        {
            // Removed !b.IsRemove due to database schema mismatch (Invalid column name 'IsRemove')
            return await _context.Books.CountAsync();
        }

        public async Task<List<RevenueDataDTO>> GetRevenueByMonthAsync(int year)
        {
            var data = await _context.Orders
                .Where(o => o.OrderDate.Year == year && o.Status != OrderStatus.Cancelled)
                .GroupBy(o => o.OrderDate.Month)
                .OrderBy(g => g.Key) // Order by Month integer directly
                .Select(g => new RevenueDataDTO
                {
                    Label = g.Key.ToString(),
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .ToListAsync();

            return data;
        }

        public async Task<List<RevenueDataDTO>> GetRevenueByDayAsync(DateTime startDate, DateTime endDate)
        {
            var data = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Status != OrderStatus.Cancelled)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return data.Select(x => new RevenueDataDTO
            {
                Label = x.Date.ToString("yyyy-MM-dd"),
                Revenue = x.Revenue
            }).ToList();
        }

        public async Task<List<TopProductDTO>> GetTopSellingBooksAsync(int count)
        {
            return await _context.OrderItems
                .Include(oi => oi.Book)
                .GroupBy(oi => oi.Book.Title)
                .Select(g => new TopProductDTO
                {
                    BookTitle = g.Key,
                    UnitsSold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Quantity * oi.PriceAtPurchase)
                })
                .OrderByDescending(x => x.UnitsSold)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<LowStockProductDTO>> GetLowStockBooksAsync(int threshold)
        {
            // Removed !b.IsRemove due to database schema mismatch
            return await _context.Books
                .Where(b => b.StockQuantity <= threshold)
                .OrderBy(b => b.StockQuantity)
                .Select(b => new LowStockProductDTO
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    StockQuantity = b.StockQuantity
                })
                .ToListAsync();
        }

        public async Task<List<RegistrationDataDTO>> GetRegistrationsByDayAsync(int days)
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-days);
            var data = await _context.UserAccounts
                .Where(u => u.CreatedAt >= startDate)
                .GroupBy(u => u.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return data.Select(x => new RegistrationDataDTO
            {
                Date = x.Date.ToString("yyyy-MM-dd"),
                Count = x.Count
            }).ToList();
        }
    }
}
