using Intelligence_Book_WEB.Models.Order;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Intelligence_Book_WEB.Services.User.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderHistoryDTO>> GetOrderHistoryAsync(string? accessToken);
        Task<OrderHistoryDTO?> GetOrderDetailsAsync(string? accessToken, int orderId);
    }
}
