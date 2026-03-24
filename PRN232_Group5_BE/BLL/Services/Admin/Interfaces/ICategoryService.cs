using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTOs.CategoryDTOs;
using DAL.Models;

namespace BLL.Services.Admin.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllAsync();

        Task<IEnumerable<CategoryResponseDto>> SearchAsync(string search);

        Task<CategoryResponseDto?> GetByIdAsync(int id);

        Task<CategoryResponseDto?> CreateAsync(CategoryCreateDto category);

        Task<bool> UpdateAsync(CategoryUpdate category);

        Task<bool> DeleteAsync(int id);
    }
}
