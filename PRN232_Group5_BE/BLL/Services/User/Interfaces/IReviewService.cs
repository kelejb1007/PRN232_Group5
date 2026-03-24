using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTOs.ReviewDTO;

namespace BLL.Services.User.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewResponseDTO>> GetReviewsByBookAsync(int bookId);
        Task<bool> CheckCanReviewAsync(int userId, int bookId);
        Task<ReviewResponseDTO> CreateReviewAsync(int userId, ReviewCreateDTO dto);
        Task<ReviewResponseDTO> UpdateReviewAsync(int userId, int reviewId, ReviewUpdateDTO dto);
        Task<bool> DeleteReviewAsync(int userId, int reviewId);
    }
}
