using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services.User.Interfaces
{
    public interface IBookServiceH
    {
        Task<List<Book>> GetAllAsync();
        Task<List<Book>> GetBestSellerAsync();
        
    }
}
