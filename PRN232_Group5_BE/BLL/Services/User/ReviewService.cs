using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Services.User.Interfaces;
using DAL.DTOs.ReviewDTO;
using DAL.Models;
using DAL.Repositories.User.Interfaces;

namespace BLL.Services.User
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, IOrderRepository orderRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<bool> CheckCanReviewAsync(int userId, int bookId)
        {
            // Kiểm tra xem đã mua sách này chưa
            var hasPurchased = await _orderRepository.HasUserPurchasedBookAsync(userId, bookId);
            if (!hasPurchased) return false;

            // Kiểm tra xem đã review chưa
            var existingReview = await _reviewRepository.GetByUserAndBookAsync(userId, bookId);
            return existingReview == null;
        }

        public async Task<ReviewResponseDTO> CreateReviewAsync(int userId, ReviewCreateDTO dto)
        {
            // Kiểm tra quyền mua hàng
            var hasPurchased = await _orderRepository.HasUserPurchasedBookAsync(userId, dto.BookId);
            if (!hasPurchased)
            {
                throw new InvalidOperationException("You must purchase the book before reviewing it.");
            }

            // Kiểm tra xem đã review chưa
            var existingReview = await _reviewRepository.GetByUserAndBookAsync(userId, dto.BookId);
            if (existingReview != null)
            {
                throw new InvalidOperationException("You have already reviewed this book.");
            }

            var review = _mapper.Map<Review>(dto);
            review.UserId = userId;
            review.CreatedAt = DateTime.Now;

            var createdReview = await _reviewRepository.AddAsync(review);
            
            // Reload to get navigation properties for mapping
            var result = await _reviewRepository.GetByIdAsync(createdReview.ReviewId);
            return _mapper.Map<ReviewResponseDTO>(result);
        }

        public async Task<bool> DeleteReviewAsync(int userId, int reviewId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null || review.UserId != userId)
            {
                return false;
            }

            return await _reviewRepository.DeleteAsync(reviewId);
        }

        public async Task<IEnumerable<ReviewResponseDTO>> GetReviewsByBookAsync(int bookId)
        {
            var reviews = await _reviewRepository.GetByBookIdAsync(bookId);
            return _mapper.Map<IEnumerable<ReviewResponseDTO>>(reviews);
        }

        public async Task<ReviewResponseDTO> UpdateReviewAsync(int userId, int reviewId, ReviewUpdateDTO dto)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
            {
                throw new KeyNotFoundException("Review not found.");
            }

            if (review.UserId != userId)
            {
                throw new UnauthorizedAccessException("You can only edit your own reviews.");
            }

            _mapper.Map(dto, review);
            // Có thể ko cần Update CreatedAt, hoặc thêm UpdatedAt nếu model có

            await _reviewRepository.UpdateAsync(review);
            
            var result = await _reviewRepository.GetByIdAsync(reviewId);
            return _mapper.Map<ReviewResponseDTO>(result);
        }
    }
}
