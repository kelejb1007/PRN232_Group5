using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.Data;
using DAL.Models;
using DAL.Models;
using BLL.Services.User.Interfaces;
using AutoMapper;
using DAL.DTOs.BookDTO;

namespace Intelligence_Book_API.Controllers.User
{

    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService_Anh _service;
        private readonly IMapper _mapper;

        public BooksController(IBookService_Anh service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
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

            return Ok(_mapper.Map<List<BookResponeDTO>>(books));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _service.GetBookById(id);

            if (book == null)
                return NotFound();

            return Ok(_mapper.Map<BookResponeDTO>(book));
        }
    }
}
