using BLL.Services.User.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intelligence_Book_API.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                userIdClaim = User.FindFirst(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub);
            }
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetOrderHistory()
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var history = await _orderService.GetOrderHistoryAsync(userId);
            return Ok(history);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var details = await _orderService.GetOrderDetailsAsync(id, userId);
            if (details == null) return NotFound("Order not found.");

            return Ok(details);
        }
    }
}
