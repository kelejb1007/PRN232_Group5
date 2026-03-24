using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.Repositories.User.Interfaces
{
    public interface IOrderRepository
    {
        Task<bool> HasUserPurchasedBookAsync(int userId, int bookId);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
        Task<Order?> GetOrderDetailsAsync(int orderId, int userId);
    }
}
