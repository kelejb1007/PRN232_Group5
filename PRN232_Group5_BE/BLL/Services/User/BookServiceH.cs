using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Services.User.Interfaces;
using DAL.Models;
using DAL.Repositories.User.Interfaces;

namespace BLL.Services.User
{
    public class BookServiceH : IBookServiceH
    {
        private readonly IBookRepositoryH _bookRepository;

        public BookServiceH(IBookRepositoryH bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<List<Book>> GetAllAsync()
        {
            return await _bookRepository.GetAllAsync();
        }
    }
}
