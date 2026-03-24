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
    public class BookRepositoryH : IBookRepositoryH
    {
        private readonly Intelligence_Book_APIContext _context;

        public BookRepositoryH(Intelligence_Book_APIContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetAllAsync()
        {
            return await _context.Books.Where(b => !b.IsRemove).ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books
                .FirstOrDefaultAsync(b => b.BookId == id && !b.IsRemove);
        }
        public async Task<List<Book>> GetBestSellerAsync()
        {
            var bestSellerIds = await _context.OrderItems
                .Where(oi => oi.Order != null && oi.Order.Status == OrderStatus.Delivered)
                .GroupBy(oi => oi.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(8)
                .Select(x => x.BookId)
                .ToListAsync();

            var books = await _context.Books
                .Where(b => bestSellerIds.Contains(b.BookId) && !b.IsRemove)
                .ToListAsync();

            // 🔥 giữ đúng thứ tự best seller
            return books
                .OrderBy(b => bestSellerIds.IndexOf(b.BookId))
                .ToList();
        }
    }
}
