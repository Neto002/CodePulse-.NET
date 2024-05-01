using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {

        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            this._categoryRepository = categoryRepository;
        }

        // POST: https://localhost:7059/api/categories
        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto request)
        {
            // Map DTO to Domain Model

            var category = new Category
            {
                Name = request.Name,
                UrlHandle = request.UrlHandle,
            };

            category = await _categoryRepository.CreateAsync(category);

            // Domain model to DTO
            var response = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle,
            };

            return Ok(response);
        }

        // GET: https://localhost:7059/api/categories
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {

            var categories = await this._categoryRepository.GetAllAsync();

            // Map Domain model to DTO
            var response = new List<CategoryDto>();

            // LINQ Expression
            categories.ToList().ForEach(category =>
            {
                response.Add(new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    UrlHandle = category.UrlHandle
                });
            });

            // foreach (var category in categories)
            // {
            //     response.Add(new CategoryDTO()
            //     {
            //         Id = category.Id,
            //         Name = category.Name,
            //         UrlHandle = category.UrlHandle
            //     });
            // }

            return Ok(response);
        }

        // GET: https://localhost:7059/api/categories/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] Guid id)
        {
            var existingCategory = await _categoryRepository.GetById(id);

            if (existingCategory == null)
            {
                return NotFound();
            }

            var response = new CategoryDto
            {
                Id = existingCategory.Id,
                Name = existingCategory.Name,
                UrlHandle = existingCategory.UrlHandle
            };

            return Ok(response);
        }

        // PUT: https://localhost:7059/api/categories/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> EditCategory([FromRoute] Guid id, [FromBody] UpdateCategoryRequestDto request)
        {
            var category = new Category
            {
                Id = id,
                Name = request.Name,
                UrlHandle = request.UrlHandle
            };

            category = await _categoryRepository.UpdateAsync(category);

            if (category == null)
            {
                return NotFound();
            }

            var response = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle
            };

            return Ok(response);
        }

        // DELETE: https://localhost:7059/api/categories/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
        {
            var category = await _categoryRepository.DeleteAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            var response = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle
            };

            return Ok(response);
        }
    }
}
