using DAL.Models;

namespace DAL.Repositories.User.Interfaces
{
    public interface IAddressRepository
    {
        Task<IEnumerable<DeliveryAddress>> GetByUserIdAsync(int userId);
        Task<DeliveryAddress?> GetByIdAsync(int addressId, int userId);
        Task<DeliveryAddress> AddAsync(DeliveryAddress address);
        Task<bool> UpdateAsync(DeliveryAddress address);
        Task<bool> DeleteAsync(int addressId, int userId);
        Task<bool> SetDefaultAsync(int addressId, int userId);
    }
}
