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
    public class ReviewRepository : IReviewRepository
    {
        private readonly Intelligence_Book_APIContext _context;

        public ReviewRepository(Intelligence_Book_APIContext context)
        {
            _context = context;
        }

        public async Task<Review> AddAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Review>> GetByBookIdAsync(int bookId)
        {
            return await _context.Reviews
                .Include(r => r.UserAccount)
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Review?> GetByIdAsync(int reviewId)
        {
            return await _context.Reviews
                .Include(r => r.UserAccount)
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId);
        }

        public async Task<Review?> GetByUserAndBookAsync(int userId, int bookId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.BookId == bookId);
        }

        public async Task<bool> UpdateAsync(Review review)
        {
            _context.Entry(review).State = EntityState.Modified;
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}
