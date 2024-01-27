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
    }
}
