using ClientProject.Model;
using IdentityAuthentication.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerProject.Models;
using ServerProject.Services;
namespace ServerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService = null;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        //[Authorize]
        [HttpGet]
        [Route("getProducts")]
        public MetaData<Product> getProducts([FromQuery] RequestParameters productParameters, [FromQuery] string filter = "")
        {
            MetaData<Product> data = _productService.GetAll(productParameters, filter);
            return data;
        }
        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        [Route("getProduct")]
        public IActionResult getProductById(int productId)
        {
            var data = _productService.GetById(productId);
            return Ok(data);
        }
        [HttpPost]
        [Route("createProduct")]
        public IActionResult createProduct(Product product)
        {
            var data = _productService.Create(product);
            return Ok(data);
        }
        [HttpDelete]
        [Route("deleteProduct")]
        public IActionResult deleteProduct(int productId)
        {
            var data = _productService.Delete(productId);
            return Ok(data);
        }
        [HttpPut]
        [Route("updateProduct")]
        public IActionResult updateProduct(Product product)
        {
            var data = _productService.Update(product);
            return Ok(data);
        }
    }
}
