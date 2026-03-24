using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.Data;
using DAL.Models;
using BLL.Services.Admin.Interfaces;
using DAL.DTOs.CategoryDTOs;

namespace Intelligence_Book_API.Controllers.Admin
{
    [Route("api/Admin/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        // GET: api/Admin/Categories?search=...
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetCategories(
            [FromQuery] string? search)
        {
            var categories = string.IsNullOrWhiteSpace(search)
                ? await _service.GetAllAsync()
                : await _service.SearchAsync(search);
            return Ok(categories);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponseDto>> GetCategory(int id)
        {
            var category = await _service.GetByIdAsync(id);

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, CategoryUpdate category)
        {
            if (id != category.CategoryId)
                return BadRequest();

            var result = await _service.UpdateAsync(category);

            if (!result)
                return NotFound();

            return NoContent();
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CategoryResponseDto>> PostCategory(CategoryCreateDto category)
        {
            var created = await _service.CreateAsync(category);

            if (created == null)
                return BadRequest("Category already exists");

            return CreatedAtAction(nameof(GetCategory),
                                   new { id = created.CategoryId },
                                   created);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }

        //private bool CategoryExists(int id)
        //{
        //    return _context.Categories.Any(e => e.CategoryId == id);
        //}
    }
}
