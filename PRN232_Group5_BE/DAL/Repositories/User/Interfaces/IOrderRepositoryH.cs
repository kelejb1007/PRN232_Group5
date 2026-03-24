using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTOs;
using DAL.Models;

namespace DAL.Repositories.User.Interfaces
{
    public interface IOrderRepositoryH
    {
        Task AddAsync(Order order);
        Task<Order?> GetByIdAsync(int orderId);
        Task UpdateAsync(Order order);
        Task<List<Order>> GetOrdersByUserId(int userId);
        Task<List<OrderListDto>> GetOrdersByUser(int userId);
    }
}
