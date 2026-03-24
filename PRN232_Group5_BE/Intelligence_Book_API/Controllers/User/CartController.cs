using BLL.Services.User.Interfaces;
using DAL.DTOs;
using Intelligence_Book_API.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intelligence_Book_API.Controllers.User
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartServiceH _service;
        private readonly PayOSService _payOS;

        public CartController(ICartServiceH service, PayOSService payOS)
        {
            _service = service;
            _payOS = payOS;
        }

        private int GetUserId()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                      ?? User.FindFirst("sub")?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException("Token không hợp lệ");

            return int.Parse(userId);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            return Ok(await _service.GetCartByUser(userId));
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] AddCartRequest request)
        {
            await _service.AddToCart(GetUserId(), request.BookId, request.Quantity);
            return Ok();
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UpdateCartRequest request)
        {
            await _service.UpdateQuantity(request.CartId, request.Quantity, GetUserId());
            return Ok();
        }

        [HttpDelete("{cartId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int cartId)
        {
            await _service.Remove(cartId, GetUserId());
            return Ok();
        }

        [HttpDelete("clear")]
        [Authorize]
        public async Task<IActionResult> ClearCart()
        {
            await _service.ClearCart(GetUserId());
            return Ok();
        }

        [HttpPost("checkout")]
        [Authorize]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest req)
        {
            try
            {
                var userId = GetUserId();

                var order = await _service.CreateOrder(
                    userId,
                    req.ShippingAddress,
                    req.ReceiverName,
                    req.PhoneNumber
                );

                var url = await _payOS.CreatePayment(order.OrderId, order.TotalAmount);

                return Ok(new
                {
                    orderId = order.OrderId,
                    checkoutUrl = url
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet("orders")]
        [Authorize]
        public async Task<IActionResult> GetOrders()
        {
            var userId = GetUserId();

            var orders = await _service.GetOrdersByUser(userId);

            return Ok(orders);
        }
    }
}