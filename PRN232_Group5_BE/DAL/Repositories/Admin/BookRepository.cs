using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data;
using DAL.Models;
using DAL.Repositories.Admin.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Admin
{
    public class BookRepository : IBookRepository
    {
        private readonly Intelligence_Book_APIContext _context;

        public BookRepository(Intelligence_Book_APIContext context)
        {
            _context = context;
        }
        public async Task<Book> CreateAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                book.IsRemove = true;
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books.Where(c => !c.IsRemove)
                .Include(b => b.Author)
                .Include(b=>b.Categories)
                .ToListAsync();
        
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
           return await _context.Books
                                    .Where(c => !c.IsRemove && c.BookId == id)
                                    .Include(c => c.Author)
                                    .Include(c => c.Categories)
                                    .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(Book book)
        {
            _context.Books.Update(book);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}
