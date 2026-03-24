using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTOs.BookDTO;
using DAL.Models;

namespace BLL.Services.Admin.Interfaces
{
    public interface IBookService
    {
        Task<List<BookResponeDTO>> GetAllAsync();
        Task<List<BookResponeDTO>> SearchAsync(string? search, List<int>? categoryIds);
        Task<BookResponeDTO?> GetByIdAsync(int id);
        Task<BookResponeDTO> CreateAsync(BookCreateDTO dto);
        Task<BookResponeDTO?> UpdateAsync(int id, BookUpdateDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
