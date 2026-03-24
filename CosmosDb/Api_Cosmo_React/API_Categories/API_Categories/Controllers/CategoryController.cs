using API_Categories.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace API_Categories.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryCosmosService _categoryService;
        public CategoryController(ICategoryCosmosService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _categoryService.GetCategories("SELECT * FROM c");
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _categoryService.GetCategories($"SELECT * FROM c WHERE c.id = '{id}'");
            var category = result.FirstOrDefault();
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Category newCategory)
        {
            if (string.IsNullOrEmpty(newCategory.CategoryID))
                newCategory.CategoryID = Guid.NewGuid().ToString();

            await _categoryService.Add(newCategory);
            return CreatedAtAction(nameof(GetById), new { id = newCategory.CategoryID }, newCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Category categoryToUpdate)
        {
            var result = await _categoryService.GetCategories($"SELECT * FROM c WHERE c.id = '{id}'");
            if (result.FirstOrDefault() == null) return NotFound();
            categoryToUpdate.CategoryID = id;
            await _categoryService.Update(categoryToUpdate);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _categoryService.GetCategories($"SELECT * FROM c WHERE c.id = '{id}'");
            var category = result.FirstOrDefault();
            if (category == null) return NotFound();
            await _categoryService.Delete(id, category.CategoryName); // partition key = CategoryName
            return NoContent();
        }
    }
}
