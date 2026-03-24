using BLL.Services.Admin.Interfaces;
using DAL.DTOs.BookDTO;
using DAL.DTOs.CategoryDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Intelligence_Book_API.Controllers.Admin
{
    [Route("api/Admin/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {

        private readonly IBookService _bookService;

        public BooksController (IBookService bookService)
        {
            this._bookService = bookService;
        }


        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<BookResponeDTO>> PostBook([FromForm] BookCreateDTO dto)
        {
            var created = await _bookService.CreateAsync(dto);

            if (created == null)
                return BadRequest("Book already exists");

            return CreatedAtAction(nameof(GetBookById),
                                   new { id = created.BookId },
                                   created);
        }


        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookResponeDTO>> GetBookById(int id)
        {
            var book = await _bookService.GetByIdAsync(id);

            if (book == null)
                return NotFound();

            return Ok(book);
        }


        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookResponeDTO>>> GetBooks()
        {
            var books = await _bookService.GetAllAsync();
            return Ok(books);
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<ActionResult<BookResponeDTO>> UpdateBook(int id, [FromForm] BookUpdateDTO dto)
        {
            var updated = await _bookService.UpdateAsync(id, dto);
            if (updated == null)
            {
                return NotFound();
            }
            return Ok(updated);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var success = await _bookService.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
