using Repository.DTO;
using Repository.Models;
using Repository.Repositories; // Import the correct namespace
using Service.IService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

namespace Service.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CategoryRepository _categoryRepository; // Use the CategoryRepository

        public CategoryService(IUnitOfWork unitOfWork, CategoryRepository categoryRepository) // Corrected constructor
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                var categoryDTOs = categories.Select(MapCategoryToDto).ToList();
                return categoryDTOs;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving categories", ex);
            }
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return null; // Or,  return new ResponseDTO(404, "Category not found.");
                }
                var categoryDTO = MapCategoryToDto(category);
                return categoryDTO;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving category by ID", ex);
            }
        }

        public async Task<CategoryDTO> AddCategoryAsync(CategoryDTO categoryDTO)
        {
            try
            {
                var category = MapDtoToCategory(categoryDTO);
               
                await _categoryRepository.CreateAsync(category);
                await _unitOfWork.SaveChangesAsync();

                categoryDTO.Id = category.Id; //update the dto
                return categoryDTO;
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding category", ex);
            }
        }

        public async Task<CategoryDTO> UpdateCategoryAsync(int id, CategoryDTO categoryDTO)
        {
            try
            {
                var existingCategory = await _categoryRepository.GetByIdAsync(id);
                if (existingCategory == null)
                {
                    throw new KeyNotFoundException($"Category with id {id} not found");
                }

                UpdateCategoryFromDto(categoryDTO, existingCategory);
                await _categoryRepository.UpdateAsync(existingCategory);
                await _unitOfWork.SaveChangesAsync();
                return categoryDTO;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating category", ex);
            }
        }

        public async Task DeleteCategoryAsync(int id)
        {
            try
            {
                var existingCategory = await _categoryRepository.GetByIdAsync(id);
                if (existingCategory == null)
                {
                    throw new KeyNotFoundException($"Category with id {id} not found");
                }
                await _categoryRepository.RemoveAsync(existingCategory);
                await _unitOfWork.SaveChangesAsync();
                return;
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting category", ex);
            }
        }



        private CategoryDTO MapCategoryToDto(Category category)
        {
            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
            };
        }

        private Category MapDtoToCategory(CategoryDTO categoryDTO)
        {
            return new Category
            {
                Id = categoryDTO.Id,
                Name = categoryDTO.Name,
                Description = categoryDTO.Description,
            };
        }

        private void UpdateCategoryFromDto(CategoryDTO categoryDTO, Category category)
        {
            category.Name = categoryDTO.Name;
            category.Description = categoryDTO.Description;
           
        }
    }
}
