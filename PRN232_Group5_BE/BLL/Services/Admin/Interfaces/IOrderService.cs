using DAL.DTOs.Admin.Orders;
using DAL.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Admin.Interfaces
{
    public interface IOrderService
    {
        Task<(List<OrderDto> items, int totalItems)> GetPagedAsync(string? search, OrderStatus? status, int page, int pageSize);
        Task<OrderDetailDto?> GetByIdAsync(int id);
        Task<bool> UpdateStatusAsync(int id, OrderUpdateStatusDto dto);
        Task<bool> CancelOrderAsync(int id);
    }
}
