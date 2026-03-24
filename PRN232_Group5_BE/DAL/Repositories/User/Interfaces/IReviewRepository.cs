using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.Repositories.User.Interfaces
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetByBookIdAsync(int bookId);
        Task<Review?> GetByUserAndBookAsync(int userId, int bookId);
        Task<Review?> GetByIdAsync(int reviewId);
        Task<Review> AddAsync(Review review);
        Task<bool> UpdateAsync(Review review);
        Task<bool> DeleteAsync(int id);
    }
}
