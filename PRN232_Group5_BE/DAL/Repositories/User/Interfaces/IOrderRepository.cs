using DAL.Models;

namespace DAL.Repositories.User.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
        Task<Order?> GetOrderDetailsAsync(int orderId, int userId);
    }
}
