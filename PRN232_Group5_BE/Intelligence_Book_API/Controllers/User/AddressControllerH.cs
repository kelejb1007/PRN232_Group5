using DAL.Repositories.User.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Intelligence_Book_API.Controllers.User
{
    [Route("api/address")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressRepository _repo;

        public AddressController(IAddressRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var data = await _repo.GetByUserIdAsync(userId);
            return Ok(data);
        }
    }
}
