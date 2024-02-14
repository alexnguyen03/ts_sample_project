using Nest;

using ServerProject.Models;
namespace WebhookServer.Service
{
    internal class ProductService
    {
        public readonly IElasticClient _elasticClient;
        public ProductService(
         IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }
        public Product AddProductToES(Product product)
        {
            var response = _elasticClient.IndexDocument(product);
            if (response.IsValid)
            {
                return product;
            }
            else
            {
                return new Product();
            }
        }
    }
}
