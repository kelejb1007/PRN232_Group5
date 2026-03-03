using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data;
using DAL.Models;
using DAL.Repositories.User.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.User
{
    public class CartRepositoryH : ICartRepositoryH
    {
        private readonly Intelligence_Book_APIContext _context;

        public CartRepositoryH(Intelligence_Book_APIContext context)
        {
            _context = context;
        }

        public async Task<List<Cart>> GetByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(x => x.Book)
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<Cart?> GetCartItemAsync(int userId, int bookId)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == bookId);
        }

        public async Task AddAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cart cart)
        {
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int cartId)
        {
            var item = await _context.Carts.FindAsync(cartId);
            if (item != null)
            {
                _context.Carts.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Cart?> GetByIdAsync(int cartId)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(x => x.CartId == cartId);
        }
    }
}
