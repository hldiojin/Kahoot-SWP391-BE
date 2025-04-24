using Repository.Models;
using Repository.Repositories;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly CategoryRepository _categoryRepository;

        public CategoryService(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreateCategoryAsync(Category category)
        {
            await _categoryRepository.AddAsync(category);
            return await _categoryRepository.SaveChangesAsync();
        }

       

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return false;

            _categoryRepository.Remove(category);
            return await _categoryRepository.SaveChangesAsync();
        }
        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            _categoryRepository.Update(category); // Update the category
            return await _categoryRepository.SaveChangesAsync(); // Save the changes
        }
    }
}


