using BLL.Services.User;
using BLL.Services.User.Interfaces;
using DAL.DTOs;
using Intelligence_Book_API.Services.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // =========================
        // GET CART BY USER
        // =========================
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            var cart = await _service.GetCartByUser(userId);
            return Ok(cart);
        }

        // =========================
        // ADD TO CART
        // =========================
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AddCartRequest request)
        {
            if (request == null)
                return BadRequest("Request is null");

            await _service.AddToCart(
                request.UserId,
                request.BookId,
                request.Quantity
            );

            return Ok(new { message = "Added to cart successfully" });
        }
        // =========================
        // UPDATE QUANTITY
        // =========================
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateCartRequest request)
        {
            await _service.UpdateQuantity(
                request.CartId,
                request.Quantity
            );

            return Ok(new { message = "Cart updated successfully" });
        }

        // =========================
        // DELETE CART ITEM
        // =========================
        [HttpDelete("{cartId}")]
        public async Task<IActionResult> Delete(int cartId)
        {
            await _service.Remove(cartId);
            return Ok(new { message = "Cart item deleted successfully" });
        }

        [HttpDelete("clear/{userId}")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            await _service.ClearCart(userId);
            return Ok(new { message = "Cart cleared successfully" });
        }
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest req)
        {
            if (req == null)
                return BadRequest(new { message = "Request is null" });

            try
            {
                var order = await _service.CreateOrder(
                    req.UserId,
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
                return StatusCode(500, new
                {
                    message = ex.Message
                });
            }
        }
        [HttpGet("order-detail/{orderId}")]
        public async Task<IActionResult> GetOrderDetail(int orderId)
        {
            var order = await _service.GetOrderDetail(orderId);

            if (order == null)
                return NotFound(new { message = "Không tìm thấy đơn hàng" });

            return Ok(order);
        }

        [HttpPost("mark-paid/{orderId}")]
        public async Task<IActionResult> MarkPaid(int orderId)
        {
            try
            {
                await _service.MarkOrderAsPaid(orderId);
                return Ok(new { message = "Cập nhật thanh toán thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}