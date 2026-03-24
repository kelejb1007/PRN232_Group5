using DAL.Data;
using DAL.Models;
using DAL.Repositories.Admin.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Admin
{
    public class UserRepository : IUserRepository
    {
        private readonly Intelligence_Book_APIContext _context;
        public UserRepository(Intelligence_Book_APIContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(UserAccount user)
        {
            await _context.UserAccounts.AddAsync(user);
            return await SaveChangesAsync();
        }

        public async Task<UserAccount?> GetByIdAsync(int id)
        {
            return await _context.UserAccounts.FindAsync(id);
        }

        public async Task<UserAccount?> GetUserByUsernameAsync(string username)
        {
            return await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
        }

        public async Task<bool> IsUserNameExistsAsync(string userName)
        {
            return await _context.UserAccounts.AnyAsync(x => x.Username == userName);
        }

        public async Task<bool> UpdateAsync(UserAccount user)
        {
            _context.UserAccounts.Update(user);
            return await SaveChangesAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
