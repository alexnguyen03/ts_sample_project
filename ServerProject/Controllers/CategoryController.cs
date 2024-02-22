using Microsoft.AspNetCore.Mvc;

using ServerProject.Models;
using ServerProject.Services;
namespace ServerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            this._categoryService = categoryService;
        }
        [HttpGet]
        [Route("getCategories")]
        public IActionResult getCategories()
        {
            var data = _categoryService.GetAll();
            return Ok(data);
        }
        [HttpPost]
        [Route("createCategory")]
        public IActionResult createCategory(Category category)
        {
            var data = _categoryService.Create(category);
            return Ok(data);
        }
        [HttpDelete]
        [Route("deleteCategory")]
        public IActionResult deleteCategory(int categoryId)
        {
            var data = _categoryService.Delete(categoryId);
            return Ok(data);
        }
        [HttpPut]
        [Route("updateCategory")]
        public IActionResult updateCategory(Category category)
        {
            var data = _categoryService.Update(category);
            return Ok(data);
        }
    }
}
