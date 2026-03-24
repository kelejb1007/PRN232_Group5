using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.Repositories.User.Interfaces
{
    public interface IOrderRepository
    {
        Task<bool> HasUserPurchasedBookAsync(int userId, int bookId);
        // Có thể thêm các method khác liên quan đến Order của User tại đây
    }
}
