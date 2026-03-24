using System;
using System.ComponentModel.DataAnnotations;

namespace Intelligence_Book_WEB.Models
{
    public class ReviewViewModel
    {
        public int ReviewId { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ReviewFormDTO
    {
        public int? ReviewId { get; set; } // Null if Create, Not null if Update

        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }
    }

    public class BookDetailViewModel
    {
        public BookViewModel Book { get; set; } = new();
        public List<ReviewViewModel> Reviews { get; set; } = new();
        public bool HasPurchased { get; set; } = false;
        public bool CanReview { get; set; } = false;
        public ReviewViewModel? UserReview { get; set; }
    }
}
