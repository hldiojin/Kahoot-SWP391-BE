using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

namespace Service.IService
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO> GetCategoryByIdAsync(int id);
        Task<CategoryDTO> AddCategoryAsync(CategoryDTO categoryDTO);
        Task<CategoryDTO> UpdateCategoryAsync(int id, CategoryDTO categoryDTO);
        Task DeleteCategoryAsync(int id);
    }
}
