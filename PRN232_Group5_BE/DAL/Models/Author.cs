using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }
        [Required, StringLength(255)]
        public string AuthorName { get; set; } = string.Empty;
        public string? Biography { get; set; }
        public List<Book> Books { get; set; } = new();
    }
}
