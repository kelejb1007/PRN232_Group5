using DAL.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using DAL.DTOs.CategoryDTOs;

namespace Intelligence_Book_API.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly Intelligence_Book_APIContext _context;
        private readonly IMapper _mapper;

        public CategoriesController(Intelligence_Book_APIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories.Where(b => b.IsRemove == false).ToListAsync();
            return Ok(_mapper.Map<List<CategoryResponseDto>>(categories));
        }
    }
}
