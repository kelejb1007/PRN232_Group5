using BLL.Services.User.Interfaces;
using DAL.DTOs.DeliveryAddress;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intelligence_Book_API.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _deliveryAddressService;

        public AddressController(IAddressService deliveryAddressService)
        {
            _deliveryAddressService = deliveryAddressService;
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

        [HttpGet]
        public async Task<IActionResult> GetMyAddresses()
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var addresses = await _deliveryAddressService.GetByUserIdAsync(userId);
            return Ok(addresses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddressById(int id)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var address = await _deliveryAddressService.GetByIdAsync(id, userId);
            if (address == null) return NotFound("Address not found.");

            return Ok(address);
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] DeliveryAddressDTO request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var newAddress = await _deliveryAddressService.AddAsync(userId, request);
            return CreatedAtAction(nameof(GetAddressById), new { id = newAddress.AddressId }, newAddress);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] DeliveryAddressDTO request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var success = await _deliveryAddressService.UpdateAsync(id, userId, request);
            if (!success) return NotFound("Address not found or update failed.");

            return Ok(new { message = "Address updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var success = await _deliveryAddressService.DeleteAsync(id, userId);
            if (!success) return NotFound("Address not found or delete failed.");

            return Ok(new { message = "Address deleted successfully" });
        }

        [HttpPut("{id}/default")]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var success = await _deliveryAddressService.SetDefaultAsync(id, userId);
            if (!success) return NotFound("Address not found or set default failed.");

            return Ok(new { message = "Default address updated successfully" });
        }
    }
}
