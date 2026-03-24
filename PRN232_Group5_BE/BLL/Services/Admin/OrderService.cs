using AutoMapper;
using BLL.Services.Admin.Interfaces;
using DAL.DTOs.Admin.Orders;
using DAL.Models;
using DAL.Models.Enums;
using DAL.Repositories.Admin.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Admin
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository repo, IMapper mapper, ILogger<OrderService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(List<OrderDto> items, int totalItems)> GetPagedAsync(string? search, OrderStatus? status, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 5;

            var (items, total) = await _repo.GetPagedAsync(search, status, page, pageSize);
            return (_mapper.Map<List<OrderDto>>(items), total);
        }

        public async Task<OrderDetailDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<OrderDetailDto>(entity);
        }

        public async Task<bool> UpdateStatusAsync(int id, OrderUpdateStatusDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            if (entity.Status == OrderStatus.Cancelled)
                throw new ArgumentException("Cannot update a cancelled order.");
            
            if (entity.Status == OrderStatus.Delivered)
                throw new ArgumentException("Order is already completed.");

            if (dto.NewStatus == OrderStatus.Cancelled)
                throw new ArgumentException("Use Cancel API to cancel orders.");

            entity.Status = dto.NewStatus;
            
            return await _repo.UpdateAsync(entity);
        }

        public async Task<bool> CancelOrderAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            if (entity.Status != OrderStatus.Pending && entity.Status != OrderStatus.Processing)
                throw new ArgumentException("Can only cancel Pending or Processing orders.");

            entity.Status = OrderStatus.Cancelled;
            
            bool result = await _repo.UpdateAsync(entity);
            if (result)
            {
                _logger.LogInformation("Order {OrderId} was cancelled at {CancelTime}", id, DateTime.Now);
            }
            return result;
        }
    }
}
