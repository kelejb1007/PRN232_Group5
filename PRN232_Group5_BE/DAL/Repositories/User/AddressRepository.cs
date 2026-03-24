using DAL.Data;
using DAL.Models;
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

        public async Task<IEnumerable<DeliveryAddress>> GetByUserIdAsync(int userId)
        {
            return await _context.DeliveryAddresses
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.IsDefault)
                .ThenByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<DeliveryAddress?> GetByIdAsync(int addressId, int userId)
        {
            return await _context.DeliveryAddresses
                .FirstOrDefaultAsync(d => d.AddressId == addressId && d.UserId == userId);
        }

        public async Task<DeliveryAddress> AddAsync(DeliveryAddress address)
        {
            // If it's the first address, make it default
            var existingCount = await _context.DeliveryAddresses.CountAsync(d => d.UserId == address.UserId);
            if (existingCount == 0)
            {
                address.IsDefault = true;
            }
            else if (address.IsDefault)
            {
                // Unset default on others
                var currentDefaults = await _context.DeliveryAddresses
                    .Where(d => d.UserId == address.UserId && d.IsDefault)
                    .ToListAsync();
                foreach (var cd in currentDefaults) cd.IsDefault = false;
            }

            _context.DeliveryAddresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<bool> UpdateAsync(DeliveryAddress address)
        {
            if (address.IsDefault)
            {
                var others = await _context.DeliveryAddresses
                    .Where(d => d.UserId == address.UserId && d.AddressId != address.AddressId && d.IsDefault)
                    .ToListAsync();
                foreach (var o in others) o.IsDefault = false;
            }

            _context.DeliveryAddresses.Update(address);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int addressId, int userId)
        {
            var address = await GetByIdAsync(addressId, userId);
            if (address == null) return false;

            _context.DeliveryAddresses.Remove(address);
            var result = await _context.SaveChangesAsync() > 0;

            if (result && address.IsDefault)
            {
                // Set another address as default if possible
                var nextAddress = await _context.DeliveryAddresses
                    .Where(d => d.UserId == userId)
                    .OrderByDescending(d => d.CreatedAt)
                    .FirstOrDefaultAsync();
                if (nextAddress != null)
                {
                    nextAddress.IsDefault = true;
                    await _context.SaveChangesAsync();
                }
            }

            return result;
        }

        public async Task<bool> SetDefaultAsync(int addressId, int userId)
        {
            var allAddresses = await _context.DeliveryAddresses
                .Where(d => d.UserId == userId)
                .ToListAsync();

            var target = allAddresses.FirstOrDefault(d => d.AddressId == addressId);
            if (target == null) return false;

            foreach (var addr in allAddresses)
            {
                addr.IsDefault = (addr.AddressId == addressId);
            }

            _context.DeliveryAddresses.UpdateRange(allAddresses);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
