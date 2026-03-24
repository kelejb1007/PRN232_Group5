using System.ComponentModel.DataAnnotations;

namespace DAL.DTOs.CategoryDTOs
{
    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "Tên thể loại là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên thể loại không vượt quá 100 ký tự.")]
        public string CategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
