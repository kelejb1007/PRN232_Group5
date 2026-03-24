using BLL.Services.User.Interfaces;
using DAL.DTOs;
using DAL.Models;
using DAL.Models.DAL.Models;
using DAL.Models.Enums;
using DAL.Repositories.User;
using DAL.Repositories.User.Interfaces;

public class CartServiceH : ICartServiceH
{
    private readonly ICartRepositoryH _repo;
    private readonly IOrderRepositoryH _orderRepo;
    private readonly IAddressRepository _addressRepo;
    private readonly IBookRepositoryH _bookRepo;

    public CartServiceH(
        ICartRepositoryH repo,
        IOrderRepositoryH orderRepo,
        IAddressRepository addressRepo,
        IBookRepositoryH bookRepo)
    {
        _repo = repo;
        _orderRepo = orderRepo;
        _addressRepo = addressRepo;
        _bookRepo = bookRepo;
    }

    // =========================
    // GET CART
    // =========================
    public async Task<List<Cart>> GetCartByUser(int userId)
    {
        return await _repo.GetByUserIdAsync(userId);
    }

    // =========================
    // ADD TO CART
    // =========================
    public async Task AddToCart(int userId, int bookId, int quantity)
    {
        if (quantity <= 0)
            throw new Exception("Quantity phải > 0");

        var book = await _bookRepo.GetByIdAsync(bookId);
        if (book == null)
            throw new Exception("Sách không tồn tại");

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

    // =========================
    // UPDATE
    // =========================
    public async Task UpdateQuantity(int cartId, int quantity, int userId)
    {
        if (quantity <= 0)
            throw new Exception("Quantity phải > 0");

        var cart = await _repo.GetByIdAsync(cartId);

        if (cart == null)
            throw new Exception("Cart not found");

        // 🔥 CHECK QUYỀN
        if (cart.UserId != userId)
            throw new Exception("Không có quyền");

        cart.Quantity = quantity;
        await _repo.UpdateAsync(cart);
    }

    // =========================
    // DELETE
    // =========================
    public async Task Remove(int cartId, int userId)
    {
        var cart = await _repo.GetByIdAsync(cartId);

        if (cart == null)
            throw new Exception("Cart not found");

        // 🔥 CHECK QUYỀN
        if (cart.UserId != userId)
            throw new Exception("Không có quyền");

        await _repo.DeleteAsync(cartId);
    }

    // =========================
    // CLEAR
    // =========================
    public async Task ClearCart(int userId)
    {
        var carts = await _repo.GetByUserIdAsync(userId);

        if (carts.Any())
        {
            await _repo.DeleteRangeAsync(carts);
        }
    }

    // =========================
    // CREATE ORDER
    // =========================
    public async Task<OrderResponseDto> CreateOrder(
        int userId,
        string shippingAddress,
        string receiverName,
        string phoneNumber)
    {
        var carts = await _repo.GetByUserIdAsync(userId);

        if (carts == null || !carts.Any())
            throw new Exception("Giỏ hàng trống");

        // 🔥 đảm bảo có Book
        foreach (var c in carts)
        {
            if (c.Book == null)
                throw new Exception("Lỗi dữ liệu sách");
        }

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

        // 🔥 CLEAR CART SAU KHI ORDER
        await _repo.DeleteRangeAsync(carts);

        return new OrderResponseDto
        {
            OrderId = order.OrderId,
            TotalAmount = order.TotalAmount
        };
    }

    // =========================
    // ORDER DETAIL
    // =========================
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

    // =========================
    // MARK PAID
    // =========================
    public async Task MarkOrderAsPaid(int orderId)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);

        if (order == null)
            throw new Exception("Không tìm thấy đơn hàng");

        if (order.Status == OrderStatus.Processing)
            return;

        order.Status = OrderStatus.Processing;
        await _orderRepo.UpdateAsync(order);
    }

    // =========================
    // BEST SELLER
    // =========================
    public async Task<List<Book>> GetBestSellerAsync()
    {
        return await _bookRepo.GetBestSellerAsync();
    }
    public async Task<List<OrderListDto>> GetOrdersByUser(int userId)
    {
        return await _orderRepo.GetOrdersByUser(userId);
    }
}