using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DAL.DTOs.BookDTO
{
    public class BookUpdateDTO
    {
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

        public string? Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho không được âm.")]
        public int StockQuantity { get; set; }

        public List<int> CategoryIds { get; set; } = new();
    }
}
