using DAL.Models;
using DAL.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Admin.Interfaces
{
    public interface IOrderRepository
    {
        Task<(List<Order> items, int totalItems)> GetPagedAsync(string? search, OrderStatus? status, int page, int pageSize);
        Task<Order?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Order order);
    }
}
