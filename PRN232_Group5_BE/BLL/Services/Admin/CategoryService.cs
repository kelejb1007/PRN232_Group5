using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Services.Admin.Interfaces;
using DAL.DTOs.CategoryDTOs;
using DAL.Models;
using DAL.Repositories.Admin.Interfaces;

namespace BLL.Services.Admin
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
            //await _repository.GetAllAsync();
        }

        public async Task<CategoryResponseDto?> GetByIdAsync(int id)
        {
            var categories = await _repository.GetByIdAsync(id);
            return _mapper.Map<CategoryResponseDto>(categories);
        }

        public async Task<CategoryResponseDto?> CreateAsync(CategoryCreateDto category)
        {
            
            var existingCategories = await _repository.GetAllAsync();

            // trùng tên hong đc
            if (existingCategories.Any(c =>
                c.CategoryName.ToLower() == category.CategoryName.ToLower()))
            {
                return null;
            }
            var createCategory = _mapper.Map<Category>(category);
            var created = await _repository.CreateAsync(createCategory);
            return _mapper.Map<CategoryResponseDto>(created);
        }

        public async Task<bool> UpdateAsync(CategoryUpdate category)
        {
            var checkNameIsExisted = await _repository.GetExistedByNameAsync(category.CategoryName, category.CategoryId);
            if (checkNameIsExisted)
            {
                return false;
            }
            var model = _mapper.Map<Category>(category);
            return await _repository.UpdateAsync(model);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
