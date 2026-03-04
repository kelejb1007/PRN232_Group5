using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Admin.Interfaces
{
    public interface IUserRepository
    {
        Task<UserAccount?> GetUserByUsernameAsync(string username);
        Task<bool> IsUserNameExistsAsync(string userName);

        Task<UserAccount?> GetByIdAsync(int id);
        Task<bool> AddAsync(UserAccount user);
        Task<bool> UpdateAsync(UserAccount user);
        Task<bool> SaveChangesAsync();
    }
}
