using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.Repositories.Admin.Interfaces
{
    public interface IAuthorRepository
    {
        Task<Author?> GetByNameAsync(string? authorName);
    }
}
