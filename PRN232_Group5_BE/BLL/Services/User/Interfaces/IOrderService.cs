using DAL.DTOs.Order;

namespace BLL.Services.User.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderHistoryDTO>> GetOrderHistoryAsync(int userId);
        Task<OrderHistoryDTO?> GetOrderDetailsAsync(int orderId, int userId);
    }
}
