using AutoMapper;
using BLL.Services.Admin.Interfaces;
using DAL.DTOs.Admin.Coupons;
using DAL.Models;
using DAL.Repositories.Admin.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Admin
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _repo;
        private readonly IMapper _mapper;

        public CouponService(ICouponRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        private static bool IsExpired(Coupon c)
            => c.ExpiryDate.HasValue && c.ExpiryDate.Value.Date < DateTime.Today;

        public async Task<(List<CouponDto> items, int totalItems)> GetPagedAsync(string? search, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 5;

            var (items, total) = await _repo.GetPagedAsync(search, page, pageSize);
            return (_mapper.Map<List<CouponDto>>(items), total);
        }

        public async Task<CouponDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<CouponDto>(entity);
        }

        public async Task<CouponDto> CreateAsync(CouponCreateDto dto)
        {
            dto.Code = (dto.Code ?? "").Trim();

            // ✅ Validate BE (bắt buộc)
            if (string.IsNullOrWhiteSpace(dto.Code)) throw new ArgumentException("Code is required.");
            if (dto.Code.Length > 50) throw new ArgumentException("Code max length is 50.");
            if (dto.DiscountPercent < 1 || dto.DiscountPercent > 100) throw new ArgumentException("Discount must be 1..100.");
            if (dto.quantity < 0 || dto.quantity > 10000) throw new ArgumentException("Quantity must be 0..10000.");
            if (dto.ExpiryDate.HasValue && dto.ExpiryDate.Value.Date < DateTime.Today) throw new ArgumentException("ExpiryDate cannot be in the past.");

            // ✅ Code unique
            if (await _repo.CodeExistsAsync(dto.Code)) throw new ArgumentException("Code already exists.");

            var entity = _mapper.Map<Coupon>(dto);

            var created = await _repo.AddAsync(entity);
            return _mapper.Map<CouponDto>(created);
        }

        public async Task<bool> UpdateAsync(int id, CouponUpdateDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            // Cho phép cập nhật lại ngày hết hạn mới để "hồi sinh" coupon
            // if (IsExpired(entity)) return false;

            // ✅ Validate
            if (dto.quantity < 0 || dto.quantity > 10000) throw new ArgumentException("Quantity must be 0..10000.");
            if (dto.ExpiryDate.HasValue && dto.ExpiryDate.Value.Date < DateTime.Today) throw new ArgumentException("ExpiryDate cannot be in the past.");

            // chỉ update 2 field
            entity.quantity = dto.quantity;
            entity.ExpiryDate = dto.ExpiryDate;

            return await _repo.UpdateAsync(entity);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            // ❌ expired => không cho delete
            if (IsExpired(entity)) return false;

            // ✅ soft delete
            entity.quantity = 0;
            entity.ExpiryDate = DateTime.Today.AddDays(-1);

            return await _repo.UpdateAsync(entity);
        }
    }
}
