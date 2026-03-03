using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Services.User.Interfaces;
using DAL.Models;
using DAL.Repositories.User.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services.User
{
    public class CartServiceH : ICartServiceH
    {
        private readonly ICartRepositoryH _repo;

        public CartServiceH(ICartRepositoryH repo)
        {
            _repo = repo;
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
    }
}
