namespace Intelligence_Book_WEB.Models
{
    public class BookViewModel
    {
        public int BookId { get; set; }
        public string? Title { get; set; }
        public decimal Price { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Description { get; set; }
    }
}
