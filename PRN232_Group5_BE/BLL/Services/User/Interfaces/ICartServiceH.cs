using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace BLL.Services.User.Interfaces
{
    public interface ICartServiceH
    {
        Task<List<Cart>> GetCartByUser(int userId);
        Task AddToCart(int userId, int bookId, int quantity);
        Task UpdateQuantity(int cartId, int quantity);
        Task Remove(int cartId);
    }
}
