using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.Repositories.User.Interfaces
{
    public interface IBookRepository
    {
        Task<List<Book>> GetBooks(
            string? search,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice);

        Task<Book?> GetBookById(int id);
    }
}
