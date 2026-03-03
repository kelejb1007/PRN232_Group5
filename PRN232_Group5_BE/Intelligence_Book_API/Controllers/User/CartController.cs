using BLL.Services.User;
using BLL.Services.User.Interfaces;
using DAL.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Intelligence_Book_API.Controllers.User
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartServiceH _service;

        public CartController(ICartServiceH service)
        {
            _service = service;
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
    }
}