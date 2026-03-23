using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data;
using DAL.Models;
using DAL.Repositories.Admin.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Admin
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly Intelligence_Book_APIContext _context;

        public CategoryRepository(Intelligence_Book_APIContext context)
        {
            _context = context;
        }

        // All
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                                 .Where(c => !c.IsRemove)
                                 .ToListAsync();
        }

        // by ID
        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                                 .Where(c => !c.IsRemove && c.CategoryId == id)
                                 .FirstOrDefaultAsync();
        }

        // create
        public async Task<Category> CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        // Update
        public async Task<bool> UpdateAsync(Category category)
        {
            var existingCategory = await _context.Categories
                                                 .FirstOrDefaultAsync(c => c.CategoryId == category.CategoryId);

            if (existingCategory == null)
                return false;

            existingCategory.CategoryName = category.CategoryName;
            existingCategory.Description = category.Description;
            await _context.SaveChangesAsync();
            return true;
        }

        // Delete (cái này là delete)
        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return false;

            category.IsRemove = true;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> GetExistedByNameAsync(String name, int id) {
            var modelIsExisted = await _context.Categories
                                 .Where(c => !c.IsRemove && c.CategoryName.Equals(name) && c.CategoryId != id)
                                 .FirstOrDefaultAsync();
            if (modelIsExisted != null)
            {
                return true;
            }
            return false;
        }

        public async Task<List<Category>?> GetListByIdAsync(List<int> id)
        {
            return await _context.Categories.Where(c => !c.IsRemove && id.Contains(c.CategoryId)).ToListAsync();
        }
    }
}
