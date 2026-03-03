using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.Repositories.User.Interfaces
{
    public interface ICartRepositoryH
    {
        Task<List<Cart>> GetByUserIdAsync(int userId);
        Task<Cart?> GetCartItemAsync(int userId, int bookId);
        Task AddAsync(Cart cart);
     

        Task<Cart> GetByIdAsync(int cartId);
        Task UpdateAsync(Cart cart);
        Task DeleteAsync(int cartId);
    }
}
