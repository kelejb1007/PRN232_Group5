using DAL.DTOs.DeliveryAddress;
using DAL.Models;

namespace BLL.Services.User.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<DeliveryAddressDTO>> GetByUserIdAsync(int userId);
        Task<DeliveryAddressDTO?> GetByIdAsync(int addressId, int userId);
        Task<DeliveryAddressDTO> AddAsync(int userId, DeliveryAddressDTO request);
        Task<bool> UpdateAsync(int addressId, int userId, DeliveryAddressDTO request);
        Task<bool> DeleteAsync(int addressId, int userId);
        Task<bool> SetDefaultAsync(int addressId, int userId);
    }
}
