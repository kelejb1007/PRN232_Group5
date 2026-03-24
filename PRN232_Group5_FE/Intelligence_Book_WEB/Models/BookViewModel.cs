using System;
using System.Collections.Generic;

namespace Intelligence_Book_WEB.Models
{
    public class BookViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ISBN { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? Publisher { get; set; }
        public DateTime? PublishDate { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Description { get; set; }
        public string? AuthorName { get; set; }
        public List<CategoryViewModel> Categories { get; set; } = new();
    }
}
