using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTOs;
using DAL.Models;

namespace BLL.Services.User.Interfaces
{
    public interface ICartServiceH
    {
        Task<List<Cart>> GetCartByUser(int userId);
        Task AddToCart(int userId, int bookId, int quantity);
        Task UpdateQuantity(int cartId, int quantity);
        Task Remove(int cartId);
        Task ClearCart(int userId);
        Task<string> Checkout(int userId, string address, string name, string phone);
        Task<OrderResponseDto> CreateOrder(
              int userId,
              string shippingAddress,
              string receiverName,
              string phoneNumber
          );
        Task<DAL.DTOs.OrderDetailDto?> GetOrderDetail(int orderId);
        Task<List<OrderListDto>> GetOrdersByUser(int userId);
        Task MarkOrderAsPaid(int orderId);
    }
}
