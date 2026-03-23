using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTOs.BookDTO;
using DAL.Models;

namespace DAL.Repositories.Admin.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();

        Task<Book?> GetByIdAsync(int id);

        Task<Book> CreateAsync(Book book);

        Task<bool> UpdateAsync(Book book);

        Task<bool> DeleteAsync(int id); // Soft delete
    }
}
