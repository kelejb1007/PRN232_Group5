using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTOs.CategoryDTOs
{
    public class CategoryCreateDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
