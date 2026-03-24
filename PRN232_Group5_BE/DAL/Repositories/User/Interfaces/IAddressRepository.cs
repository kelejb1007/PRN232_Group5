using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models.DAL.Models;

namespace DAL.Repositories.User.Interfaces
{
    public interface IAddressRepository
    {
        Task<DeliveryAddress?> GetByIdAsync(int id);
        Task<List<DeliveryAddress>> GetByUserIdAsync(int userId);
        Task AddAsync(DeliveryAddress address);
    }
}
