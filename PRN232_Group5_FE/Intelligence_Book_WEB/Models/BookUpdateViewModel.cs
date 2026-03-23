using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Intelligence_Book_WEB.Models
{
    public class BookUpdateViewModel
    {
        public int BookId { get; set; }

        [Required(ErrorMessage = "Tiêu đề sách là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Tiêu đề không quá 255 ký tự.")]
        public string Title { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sách phải lớn hơn 0.")]
        public decimal Price { get; set; }

        [MaxLength(20, ErrorMessage = "ISBN không quá 20 ký tự.")]
        public string? ISBN { get; set; }

        [MaxLength(255, ErrorMessage = "Tên tác giả không quá 255 ký tự.")]
        public string? AuthorName { get; set; }

        public string? Publisher { get; set; }
        public DateTime? PublishDate { get; set; }

        public IFormFile? CoverImageUrl { get; set; }
        public string? ExistingCoverImageUrl { get; set; }

        public string? Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho không được âm.")]
        public int StockQuantity { get; set; }

        public List<int> CategoryIds { get; set; } = new();

        // Used for rendering category options in the UI
        public List<CategoryViewModel> AvailableCategories { get; set; } = new();
    }
}
