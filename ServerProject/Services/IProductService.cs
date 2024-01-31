using ClientProject.Model;
using Microsoft.AspNetCore.Mvc;
using ServerProject.Models;
namespace ServerProject.Services
{
    public interface IProductService
    {
        public MetaData<Product> GetAll(RequestParameters productParameters, [FromQuery] string filter = "");
        public Product GetById(int id);
        public Product Create(Product product);
        public Product Update(Product product);
        public Product Delete(int productId);
        public Product AddProductToES(Product product);
        public void DeleteProduct(int productId);
        public List<Product> GetAllProducts();
        public Product GetProductById(int productId);
        public List<ProductElastic> SearchProduct(string keyWord);

        public void GenerateDataIntoDB();

        //public Task<ProductElastic> CreateInElastic(ProductElastic productElastic);
        //public Task<ProductElastic> UpdateInElastic(ProductElastic productElastic);
        //public Task<List<ProductElastic>> GetAsync();
        //public Task<ProductElastic?> GetAsync(int id);
        //public Task CreateAsync(ProductElastic newProduct);
        //public Task UpdateAsync(string id, ProductElastic updatedProduct);
        //public Task RemoveAsync(int id);
    }
}
