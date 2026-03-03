using Microsoft.AspNetCore.Mvc;

namespace Intelligence_Book_API.Controllers.User
{
    using Microsoft.AspNetCore.Mvc;
    using BLL.Services.User.Interfaces;

    namespace Intelligence_Book_API.Controllers.User
    {
        [Route("api/home")]
        [ApiController]
        public class HomeController : ControllerBase
        {
            private readonly IBookServiceH _bookService;

            public HomeController(IBookServiceH bookService)
            {
                _bookService = bookService;
            }

            [HttpGet("featured-books")]
            public async Task<IActionResult> GetFeaturedBooks()
            {
                var books = await _bookService.GetAllAsync();
                return Ok(books);
            }
        }
    }
}