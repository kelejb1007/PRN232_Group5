using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.Repositories.User.Interfaces
{
    public interface IBookRepositoryH
    {
        Task<List<Book>> GetAllAsync();
        Task<List<Book>> GetBestSellerAsync();

        Task<Book?> GetByIdAsync(int id);

    }
}
