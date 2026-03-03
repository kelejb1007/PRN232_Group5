using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using DAL.Repositories.User.Interfaces;

namespace BLL.Services.User.Interfaces
{
    public class BookService_Anh : IBookService_Anh
    {
        private readonly IBookRepository_Anh _repository;

        public BookService_Anh(IBookRepository_Anh repository)
        {
            _repository = repository;
        }

        public async Task<List<Book>> GetBooks(
            string? search,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice)
        {
            return await _repository.GetBooks(
                search, categoryId, minPrice, maxPrice);
        }

        public async Task<Book?> GetBookById(int id)
        {
            return await _repository.GetBookById(id);
        }
    }
}
