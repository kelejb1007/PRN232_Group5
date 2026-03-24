using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required, StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public bool IsRemove { get; set; }
        public List<Book> Books { get; set; } = new();
    }
}