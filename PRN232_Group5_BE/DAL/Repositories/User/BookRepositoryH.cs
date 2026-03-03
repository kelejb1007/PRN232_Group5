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
    public class BookRepositoryH : IBookRepositoryH
    {
        private readonly Intelligence_Book_APIContext _context;

        public BookRepositoryH(Intelligence_Book_APIContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetAllAsync()
        {
            return await _context.Books.ToListAsync();
        }
    }
}
