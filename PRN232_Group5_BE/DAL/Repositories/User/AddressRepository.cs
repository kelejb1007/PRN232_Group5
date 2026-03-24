using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data;
using DAL.Models.DAL.Models;
using DAL.Repositories.User.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.User
{
    public class AddressRepository : IAddressRepository
    {
        private readonly Intelligence_Book_APIContext _context;

        public AddressRepository(Intelligence_Book_APIContext context)
        {
            _context = context;
        }

        public async Task<DeliveryAddress?> GetByIdAsync(int id)
        {
            return await _context.DeliveryAddresses.FindAsync(id);
        }

        public async Task<List<DeliveryAddress>> GetByUserIdAsync(int userId)
        {
            return await _context.DeliveryAddresses
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task AddAsync(DeliveryAddress address)
        {
            _context.DeliveryAddresses.Add(address);
            await _context.SaveChangesAsync();
        }
    }
}
