using DAL.DTOs;
using DAL.Models;

namespace BLL.Services.User.Interfaces
{
    public interface ICartServiceH
    {
        Task<List<Cart>> GetCartByUser(int userId);

        Task AddToCart(int userId, int bookId, int quantity);

        Task UpdateQuantity(int cartId, int quantity, int userId);

        Task Remove(int cartId, int userId);

        Task ClearCart(int userId);

        Task<OrderResponseDto> CreateOrder(
            int userId,
            string shippingAddress,
            string receiverName,
            string phoneNumber
        );

        Task<OrderDetailDto?> GetOrderDetail(int orderId);

        Task MarkOrderAsPaid(int orderId);

        Task<List<Book>> GetBestSellerAsync();

        Task<List<OrderListDto>> GetOrdersByUser(int userId);
    }
}