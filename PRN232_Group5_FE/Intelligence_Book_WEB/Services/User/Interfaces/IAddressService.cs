using Intelligence_Book_WEB.Models.DeliveryAddress;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Intelligence_Book_WEB.Services.User.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<DeliveryAddressDTO>> GetMyAddressesAsync(string? accessToken);
        Task<bool> AddAddressAsync(string? accessToken, DeliveryAddressDTO addressRequest);
        Task<bool> UpdateAddressAsync(string? accessToken, int addressId, DeliveryAddressDTO addressRequest);
        Task<bool> DeleteAddressAsync(string? accessToken, int addressId);
        Task<bool> SetDefaultAddressAsync(string? accessToken, int addressId);
    }
}
