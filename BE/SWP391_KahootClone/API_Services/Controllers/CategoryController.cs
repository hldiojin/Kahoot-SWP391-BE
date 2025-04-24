
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Service.IService;
using Service.IServices;
using Service.Service;
namespace API_Services.Controllers
{
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _CategoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _CategoryService = categoryService;
        }
        /// <summary>
        /// Lấy tất cả danh mục
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _CategoryService.GetAllCategoriesAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh mục theo Id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _CategoryService.GetCategoryByIdAsync(id);
            if (result == null) return NotFound("Category not found.");
            return Ok(result);
        }

        /// <summary>
        /// Tạo danh mục mới
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] Category category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _CategoryService.CreateCategoryAsync(category);
            if (!success)
                return BadRequest("Tạo danh mục thất bại.");

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        /// <summary>
        /// Cập nhật danh mục
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Category category)
        {
            if (id != category.Id)
                return BadRequest("ID không khớp.");

            var existing = await _CategoryService.GetCategoryByIdAsync(id);
            if (existing == null) return NotFound("Không tìm thấy danh mục.");

            var success = await _CategoryService.UpdateCategoryAsync(category);
            if (!success)
                return BadRequest("Cập nhật thất bại.");

            return NoContent();
        }

        /// <summary>
        /// Xóa danh mục
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _CategoryService.DeleteCategoryAsync(id);
            if (!success)
                return NotFound("Không tìm thấy danh mục để xóa.");

            return NoContent();
        }
    }
}

