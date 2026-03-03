using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.Data;
using DAL.Models;
using BLL.Services.User.Interfaces;

namespace Intelligence_Book_API.Controllers.User
{

    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService_Anh _service;

        public BooksController(IBookService_Anh service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks(
            string? search,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice)
        {
            var books = await _service.GetBooks(
                search, categoryId, minPrice, maxPrice);

            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _service.GetBookById(id);

            if (book == null)
                return NotFound();

            return Ok(book);
        }
    }
}
