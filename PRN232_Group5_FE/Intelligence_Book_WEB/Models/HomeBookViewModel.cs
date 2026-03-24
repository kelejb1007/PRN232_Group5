namespace Intelligence_Book_WEB.Models
{
        public class HomeBookViewModel
        {
            public int BookId { get; set; }
            public string Title { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public string? CoverImageUrl { get; set; }
            public string? AuthorName { get; set; }
        }
    }