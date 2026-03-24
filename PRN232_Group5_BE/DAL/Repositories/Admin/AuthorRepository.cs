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
    public class AuthorRepository : IAuthorRepository
    {
        private readonly Intelligence_Book_APIContext _context;

        public AuthorRepository(Intelligence_Book_APIContext context)
        {
            _context = context;
        }
        public async Task<Author?> GetByNameAsync(string? authorName)
        {
            if (string.IsNullOrEmpty(authorName)) return null;
            return await _context.Authors
                                 .FirstOrDefaultAsync(c => c.AuthorName.ToLower() == authorName.ToLower());
        }

    }
}
