﻿using ClientProject.Model;

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
        private readonly IMessageProducer _messagePublisher;
        public ProductController(ILogger<ProductController> logger, IProductService productService, IMessageProducer messagePublisher)
        {
            _productService = productService;
            _logger = logger;
            _messagePublisher = messagePublisher;
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
        //[Authorize(Roles = UserRoles.Admin)]
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
        [Route("api/AddProductIntoES")]
        public void AddProduct([FromBody] ProductElastic productElastic)
        {
            _productService.AddProductToES(productElastic);
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
        [HttpPost]
        [Route("api/UpdateProductInES")]
        public ProductElastic UpdateProduct([FromBody] ProductElastic productElastic)
        {
            return _productService.UpdateInElastic(productElastic);
        }
    }
}
