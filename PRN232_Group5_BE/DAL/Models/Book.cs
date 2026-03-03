using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        [Required, StringLength(255)]
        public string Title { get; set; } = string.Empty;
        public string? ISBN { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; } = 0;
        public string? Publisher { get; set; }
        public DateTime? PublishDate { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsRemove { get; set; }
        public int? AuthorId { get; set; }
        public Author? Author { get; set; }
        public List<Category> Categories { get; set; } = new();
    }
}
