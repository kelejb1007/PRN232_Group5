using BLL.Services.User.Interfaces;
using DAL.DTOs;
using DAL.Models;
using DAL.Models.DAL.Models;
using DAL.Models.Enums;
using DAL.Repositories.User.Interfaces;

namespace BLL.Services.User
{
    public class CartServiceH : ICartServiceH
    {
        private readonly ICartRepositoryH _repo;
        private readonly IOrderRepositoryH _orderRepo;
        private readonly IAddressRepository _addressRepo;

        public CartServiceH(
            ICartRepositoryH repo,
            IOrderRepositoryH orderRepo,
            IAddressRepository addressRepo)
        {
            _repo = repo;
            _orderRepo = orderRepo;
            _addressRepo = addressRepo;
        }

        public async Task<List<Cart>> GetCartByUser(int userId)
        {
            return await _repo.GetByUserIdAsync(userId);
        }

        public async Task AddToCart(int userId, int bookId, int quantity)
        {
            var existingItem = await _repo.GetCartItemAsync(userId, bookId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                await _repo.UpdateAsync(existingItem);
            }
            else
            {
                var cart = new Cart
                {
                    UserId = userId,
                    BookId = bookId,
                    Quantity = quantity,
                    AddedAt = DateTime.Now
                };

                await _repo.AddAsync(cart);
            }
        }

        public async Task UpdateQuantity(int cartId, int quantity)
        {
            var cart = await _repo.GetByIdAsync(cartId);

            if (cart == null)
                throw new Exception("Cart not found");

            cart.Quantity = quantity;
            await _repo.UpdateAsync(cart);
        }

        public async Task Remove(int cartId)
        {
            await _repo.DeleteAsync(cartId);
        }

        public async Task ClearCart(int userId)
        {
            var carts = await _repo.GetByUserIdAsync(userId);

            if (carts != null && carts.Any())
            {
                await _repo.DeleteRangeAsync(carts);
            }
        }

        public async Task<string> Checkout(int userId, string address, string name, string phone)
        {
            var carts = await _repo.GetByUserIdAsync(userId);

            if (carts == null || !carts.Any())
                throw new Exception("Cart empty");

            var newAddress = new DeliveryAddress
            {
                UserId = userId,
                ReceiverName = name,
                PhoneNumber = phone,
                AddressLine = address,
                CreatedAt = DateTime.Now
            };

            await _addressRepo.AddAsync(newAddress);

            decimal total = carts.Sum(x => x.Quantity * x.Book.Price);

            var order = new Order
            {
                UserId = userId,
                ShippingAddress = address,
                TotalAmount = total,
                Status = OrderStatus.Pending,
                OrderItems = carts.Select(c => new OrderItem
                {
                    BookId = c.BookId,
                    Quantity = c.Quantity,
                    PriceAtPurchase = c.Book.Price
                }).ToList()
            };

            await _orderRepo.AddAsync(order);

            return $"https://pay.payos.vn/checkout/{order.OrderId}";
        }

        public async Task<OrderResponseDto> CreateOrder(int userId, string shippingAddress, string receiverName, string phoneNumber)
        {
            var carts = await _repo.GetByUserIdAsync(userId);

            if (carts == null || !carts.Any())
                throw new Exception("Giỏ hàng trống");

            var newAddress = new DeliveryAddress
            {
                UserId = userId,
                ReceiverName = receiverName,
                PhoneNumber = phoneNumber,
                AddressLine = shippingAddress,
                CreatedAt = DateTime.Now
            };

            await _addressRepo.AddAsync(newAddress);

            decimal total = carts.Sum(x => x.Quantity * x.Book.Price);

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                ShippingAddress = shippingAddress,
                TotalAmount = total,
                Status = OrderStatus.Pending,
                OrderItems = carts.Select(c => new OrderItem
                {
                    BookId = c.BookId,
                    Quantity = c.Quantity,
                    PriceAtPurchase = c.Book.Price
                }).ToList()
            };

            await _orderRepo.AddAsync(order);

            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                TotalAmount = order.TotalAmount
            };
        }

        public async Task<OrderDetailDto?> GetOrderDetail(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);

            if (order == null)
                return null;

            return new OrderDetailDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                ShippingAddress = order.ShippingAddress,
                Items = order.OrderItems.Select(x => new OrderDetailItemDto
                {
                    OrderItemId = x.OrderItemId,
                    BookId = x.BookId,
                    BookTitle = x.Book?.Title ?? "Không có tên sách",
                    Quantity = x.Quantity,
                    PriceAtPurchase = x.PriceAtPurchase
                }).ToList()
            };
        }
        public async Task<List<OrderListDto>> GetOrdersByUser(int userId)
        {
            var orders = await _orderRepo.GetOrdersByUserId(userId);

            return orders.Select(o => new OrderListDto
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                ShippingAddress = o.ShippingAddress,
                TotalItems = o.OrderItems != null ? o.OrderItems.Sum(i => i.Quantity) : 0
            }).ToList();
        }
        public async Task MarkOrderAsPaid(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);

            if (order == null)
                throw new Exception("Không tìm thấy đơn hàng");

            if (order.Status == OrderStatus.Processing)
                return;

            order.Status = OrderStatus.Processing;
            await _orderRepo.UpdateAsync(order);

            if (order.UserId.HasValue)
            {
                var carts = await _repo.GetByUserIdAsync(order.UserId.Value);

                if (carts != null && carts.Any())
                {
                    await _repo.DeleteRangeAsync(carts);
                }
            }
        }
    }
}