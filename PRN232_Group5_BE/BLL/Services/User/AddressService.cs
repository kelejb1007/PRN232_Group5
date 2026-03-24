using BLL.Services.User.Interfaces;
using DAL.DTOs.DeliveryAddress;
using DAL.Models;
using DAL.Repositories.User.Interfaces;

namespace BLL.Services.User
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepo;

        public AddressService(IAddressRepository addressRepo)
        {
            _addressRepo = addressRepo;
        }

        private DeliveryAddressDTO MapToDTO(DeliveryAddress entity)
        {
            return new DeliveryAddressDTO
            {
                AddressId = entity.AddressId,
                ReceiverName = entity.ReceiverName,
                PhoneNumber = entity.PhoneNumber,
                AddressLine = entity.AddressLine,
                IsDefault = entity.IsDefault
            };
        }

        public async Task<IEnumerable<DeliveryAddressDTO>> GetByUserIdAsync(int userId)
        {
            var entities = await _addressRepo.GetByUserIdAsync(userId);
            return entities.Select(MapToDTO);
        }

        public async Task<DeliveryAddressDTO?> GetByIdAsync(int addressId, int userId)
        {
            var entity = await _addressRepo.GetByIdAsync(addressId, userId);
            return entity == null ? null : MapToDTO(entity);
        }

        public async Task<DeliveryAddressDTO> AddAsync(int userId, DeliveryAddressDTO request)
        {
            var newAddress = new DeliveryAddress
            {
                UserId = userId,
                ReceiverName = request.ReceiverName?.Trim() ?? string.Empty,
                PhoneNumber = request.PhoneNumber?.Trim() ?? string.Empty,
                AddressLine = request.AddressLine?.Trim() ?? string.Empty,
                IsDefault = request.IsDefault,
                CreatedAt = DateTime.Now
            };

            var added = await _addressRepo.AddAsync(newAddress);
            return MapToDTO(added);
        }

        public async Task<bool> UpdateAsync(int addressId, int userId, DeliveryAddressDTO request)
        {
            var existing = await _addressRepo.GetByIdAsync(addressId, userId);
            if (existing == null) return false;

            existing.ReceiverName = request.ReceiverName?.Trim() ?? existing.ReceiverName;
            existing.PhoneNumber = request.PhoneNumber?.Trim() ?? existing.PhoneNumber;
            existing.AddressLine = request.AddressLine?.Trim() ?? existing.AddressLine;
            existing.IsDefault = request.IsDefault;

            return await _addressRepo.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(int addressId, int userId)
        {
            return await _addressRepo.DeleteAsync(addressId, userId);
        }

        public async Task<bool> SetDefaultAsync(int addressId, int userId)
        {
            return await _addressRepo.SetDefaultAsync(addressId, userId);
        }
    }
}
