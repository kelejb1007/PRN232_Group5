using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTOs.CategoryDTOs;

namespace DAL.DTOs.BookDTO
{
    public class BookResponeDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ISBN { get; set; }
        public string? AuthorName { get; set; }
        public string? Publisher { get; set; }
        public DateTime? PublishDate { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Description { get; set; }
        public int StockQuantity { get; set; }
        public List<CategoryResponseDto> Categories { get; set; } = new();
    }
}
