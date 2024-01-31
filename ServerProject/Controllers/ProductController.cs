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

        private readonly ILogger<ProductController> _logger;
        public ProductController(ILogger<ProductController> logger, IProductService productService)
        {
            _productService = productService;
            _logger = logger;
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



        [HttpGet]
        [Route("/[controller]/[action]")]
        public IEnumerable<Product> GetProducts()
        {
            return _productService.GetAllProducts();
        }

        [HttpGet]
        [Route("/[controller]/[action]/{id}")]
        public Product GetProductById(int id)
        {
            return _productService.GetProductById(id);
        }

        [HttpPost]
        [Route("api/AddProduct")]
        public void AddProduct([FromBody] Product product)
        {
            _productService.AddProductToES(product);
        }


        [HttpGet]
        [Route("api/SearchProduct")]
        public List<ProductElastic> SearchProduct(string keyWord)
        {
            return _productService.SearchProduct(keyWord);
        }

        [HttpGet]
        [Route("api/GenerateDataIntoDB")]
        public IActionResult GenerateDataIntoDB()
        {
            _productService.GenerateDataIntoDB();
            return Ok();
        }




        //[HttpPost]
        //[Route("api/UpdateProduct")]
        //public void UpdateProduct([FromBody] Product product)
        //{
        //    _productService.UpdateProductToES(product);
        //}

        //[HttpDelete]
        //[Route("api/DeleteByProductId/{id}")]
        //public void Delete(int id)
        //{
        //    _productService.DeleteProductInES(id);
        //}

        //[HttpGet]
        //public async Task<List<ProductElastic>> Get() =>
        //await _productService.GetAsync();
        //[HttpGet("{id:length(24)}")]
        //public async Task<ActionResult<ProductElastic>> Get(int id)
        //{
        //    var product = await _productService.GetAsync(id);
        //    if (product is null)
        //    {
        //        return NotFound();
        //    }
        //    return product;
        //}
        //[HttpPost("createProductInMDB")]
        //public async Task<IActionResult> Post(ProductElastic newProduct)
        //{
        //    await _productService.CreateAsync(newProduct);
        //    return CreatedAtAction(nameof(Get), new { id = newProduct.ProductId }, newProduct);
        //}
        //[HttpPut("{id:length(24)}")]
        //public async Task<IActionResult> Update(int id, ProductElastic updatedProduct)
        //{
        //    var product = await _productService.GetAsync(id);
        //    if (product is null)
        //    {
        //        return NotFound();
        //    }
        //    updatedProduct.Id = product.Id;
        //    await _productService.UpdateAsync(updatedProduct.Id.ToString(), updatedProduct);
        //    return NoContent();
        //}
        //[HttpDelete("{id:length(24)}")]
        //public async Task<IActionResult> DeleteInMongoDb(int id)
        //{
        //    var product = await _productService.GetAsync(id);
        //    if (product is null)
        //    {
        //        return NotFound();
        //    }
        //    await _productService.RemoveAsync(id);
        //    return NoContent();
        //}
    }
}
