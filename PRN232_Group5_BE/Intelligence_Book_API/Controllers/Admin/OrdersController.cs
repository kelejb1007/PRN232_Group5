using BLL.Services.Admin.Interfaces;
using DAL.DTOs.Admin.Common;
using DAL.DTOs.Admin.Orders;
using DAL.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Intelligence_Book_API.Controllers.Admin
{
    [Route("api/admin/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(string? search, OrderStatus? status, int page = 1, int pageSize = 5)
        {
            var (items, totalItems) = await _service.GetPagedAsync(search, status, page, pageSize);

            var result = new PagedResultDto<OrderDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                Items = items
            };

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var order = await _service.GetByIdAsync(id);
            if (order == null) return NotFound(new { message = "Order not found." });
            return Ok(order);
        }

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderUpdateStatusDto dto)
        {
            try
            {
                var ok = await _service.UpdateStatusAsync(id, dto);
                if (!ok) return BadRequest(new { message = "Update failed (not found)." });
                return Ok(new { message = "Status updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var ok = await _service.CancelOrderAsync(id);
                if (!ok) return BadRequest(new { message = "Cancel failed (not found)." });
                return Ok(new { message = "Order cancelled successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
