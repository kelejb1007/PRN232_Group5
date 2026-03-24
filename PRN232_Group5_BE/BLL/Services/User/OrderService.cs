using BLL.Services.User.Interfaces;
using DAL.DTOs.Order;
using DAL.Models;
using DAL.Repositories.User.Interfaces;

namespace BLL.Services.User
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;

        public OrderService(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public async Task<IEnumerable<OrderHistoryDTO>> GetOrderHistoryAsync(int userId)
        {
            var orders = await _orderRepo.GetOrdersByUserIdAsync(userId);
            return orders.Select(o => new OrderHistoryDTO
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                ShippingAddress = o.ShippingAddress,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDTO
                {
                    OrderItemId = oi.OrderItemId,
                    BookId = oi.BookId,
                    BookTitle = oi.Book?.Title ?? "Unknown Book",
                    BookImage = oi.Book?.CoverImageUrl,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.Book.Price
                }).ToList()
            });
        }

        public async Task<OrderHistoryDTO?> GetOrderDetailsAsync(int orderId, int userId)
        {
            var o = await _orderRepo.GetOrderDetailsAsync(orderId, userId);
            if (o == null) return null;

            return new OrderHistoryDTO
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                ShippingAddress = o.ShippingAddress,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDTO
                {
                    OrderItemId = oi.OrderItemId,
                    BookId = oi.BookId,
                    BookTitle = oi.Book?.Title ?? "Unknown Book",
                    BookImage = oi.Book?.CoverImageUrl,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.Book.Price
                }).ToList()
            };
        }
    }
}
