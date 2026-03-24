using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data;
using DAL.Models;
using DAL.Repositories.User.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.User
{
    public class BookRepository_Anh : IBookRepository_Anh
    {
        private readonly Intelligence_Book_APIContext _context;

        public BookRepository_Anh(Intelligence_Book_APIContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetBooks(
            string? search,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice)
        {
            var query = _context.Books
                .Include(b => b.Categories)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(b => b.Title.Contains(search));

            if (categoryId.HasValue)
                query = query.Where(b =>
                    b.Categories.Any(c => c.CategoryId == categoryId));

            if (minPrice.HasValue)
                query = query.Where(b => b.Price >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(b => b.Price <= maxPrice);

            return await query.ToListAsync();
        }

        public async Task<Book?> GetBookById(int id)
        {
            return await _context.Books
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(b => b.BookId == id);
        }
    }
}
