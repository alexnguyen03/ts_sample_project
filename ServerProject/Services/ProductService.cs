using ClientProject.Model;

using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

using MongoDB.Driver;

using Nest;

using ServerProject.Models;

namespace ServerProject.Services
{
    public class ProductService : IProductService
    {
        private readonly MsdemoContext dbContext = null;
        private readonly IMongoCollection<ProductElastic>? _productsCollection;
        public readonly IElasticClient _elasticClient;
        private readonly IMessageProducer _messagePublisher;
        public ProductService(
            IMessageProducer messagePublisher,
            MsdemoContext dbContext,
         IElasticClient elasticClient)
        {
            this.dbContext = dbContext;
            _elasticClient = elasticClient;
            _messagePublisher = messagePublisher;
        }


        public ProductService(
         IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }


        //public ProductService(MsdemoContext dbContext)
        //{
        //    this.dbContext = dbContext;
        //}
        public Product Create(Product product)
        {
            try
            {
                Category foundCategory = dbContext.Categories.Find(product.CategoryId)!;
                var foundProductWithSupplier = dbContext.Products
                    .Include(od => od.Supplier)
                    .FirstOrDefault(od => od.ProductId == product.ProductId);
                Supplier supplier = foundProductWithSupplier?.Supplier!;
                product.Category = foundCategory;
                product.Supplier = supplier;
                dbContext.Products.Update(product);
                dbContext.SaveChanges();
                _messagePublisher.SendMessage(product, Channel.PRODUCT.ToString());
                return product;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while creating the product.", ex)!;
            }
        }
        public Product Delete(int productId)
        {
            try
            {
                Product foundProduct = dbContext.Products.Find(productId)!;
                if (foundProduct == null)
                {
                    throw new Exception("Product not found !!!")!;
                }
                dbContext.Products.Remove(foundProduct);
                dbContext.SaveChanges();
                return foundProduct;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while deleting the product.", ex)!;
            }
        }
        public MetaData<Product> GetAll(RequestParameters productParameters, [FromQuery] string filter = "")
        {
            var query = dbContext.Products
                                 .Include(p => p.Category)
                                 .Include(p => p.Supplier)
                                 .AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(prod => prod.ProductName.Contains(filter) || prod.Supplier!.ContactName!.Contains(filter));
            }
            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / productParameters.PageSize);
            query = query.Skip((productParameters.PageNumber - 1) * productParameters.PageSize).Take(productParameters.PageSize);
            MetaData<Product> response = new MetaData<Product>();
            response.TotalCount = totalCount;
            response.CurrentPage = productParameters.PageNumber;
            response.TotalPages = totalPages;
            response.PageSize = productParameters.PageSize;
            List<Product> products = query.ToList();
            response.Data = products;
            return response;
        }
        public Product GetById(int ProductId)
        {
            return dbContext.Products
                .Include(o => o.Category)
                .Include(o => o.Units)
                .FirstOrDefault(o => o.ProductId == ProductId);
        }
        public Product Update(Product product)
        {
            try
            {
                Product foundProduct = dbContext.Products.Find(product.ProductId)!;
                Category foundCategory = dbContext.Categories.Find(product.CategoryId)!;
                var foundProductWithSupplier = dbContext.Products
                    .Include(od => od.Supplier)
                    .FirstOrDefault(od => od.ProductId == product.ProductId);
                Supplier supplier = new Supplier();
                if (foundProductWithSupplier != null)
                {
                    supplier = foundProductWithSupplier!.Supplier!;
                    foundProduct.SupplierId = product.SupplierId;
                }
                if (foundCategory != null)
                {
                    foundProduct.Category = foundCategory;
                }
                if (foundProduct == null)
                {
                    throw new Exception("Product not found !!!")!;
                }
                foundProduct.ProductName = product.ProductName;
                foundProduct.Supplier = supplier;
                foundProduct.QuantityPerUnit = product.QuantityPerUnit;
                foundProduct.UnitPrice = product.UnitPrice;
                foundProduct.UnitsInStock = product.UnitsInStock;
                foundProduct.UnitsOnOrder = product.UnitsOnOrder;
                foundProduct.ReorderLevel = product.ReorderLevel;
                foundProduct.Discontinued = product.Discontinued;
                dbContext.Products.Update(foundProduct);
                dbContext.SaveChanges();
                _messagePublisher.SendMessage(product, Channel.PRODUCT.ToString());
                return foundProduct;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while update the product.", ex)!;
            }
        }
        public ProductElastic AddProductToES(ProductElastic productElastic)
        {
            var response = _elasticClient.IndexDocument<ProductElastic
>(productElastic);
            if (response.IsValid)
            {
                return productElastic;
            }
            else
            {
                return new ProductElastic();
            }

        }
        public void DeleteProduct(int productId)
        {
            _elasticClient.DeleteByQuery<Product>(p => p.Query(q1 => q1
                             .Match(m => m
                                 .Field(f => f.ProductId)
                                 .Query(productId.ToString()
                                 )
                         )));
        }
        public List<Product> GetAllProducts()
        {
            var esResponse = _elasticClient.Search<Product>().Documents;
            return esResponse.ToList();
        }
        public Product GetProductById(int productId)
        {
            var esResponse = _elasticClient.Search<Product>(x => x.
                             Query(q1 => q1.Bool(b => b.Must(m =>
                             m.Terms(t => t.Field(f => f.ProductId)
                             .Terms<int>(productId))))));
            return esResponse.Documents.FirstOrDefault();
        }
        public List<ProductElastic> SearchProduct(string keyWord)
        {
            var searchResponse = _elasticClient.Search<ProductElastic>(s => s
             .Query(q => q
             //get matching 
             .Match(m => m
                 .Field(f => f.ProductName)
                 .Query(keyWord)
             )
             // get just using a little character
             //.Prefix(c => c
             //   //.Boost(1.1)
             //   .Field(p => p.ProductName)
             //   .Value(keyWord)
             //       )
             )
             .Size(10)
         );
            if (searchResponse.IsValid)
            {
                return searchResponse.Documents.ToList();
            }
            else
            {
                return null;
            }
        }
        public async void GenerateDataIntoDB()
        {
            List<ProductElastic> productInESs = new List<ProductElastic>();
            for (int i = 93; i <= 1014; i++)
            {
                Product pr = new Product();
                pr.ProductId = i;
                pr.ProductName = Faker.Name.FullName();
                pr.SupplierId = 3;
                pr.CategoryId = 3;
                pr.QuantityPerUnit = "48 - 6 oz jars";
                pr.UnitPrice = 30;
                pr.UnitsInStock = 39;
                pr.UnitsOnOrder = 0;
                pr.ReorderLevel = 10;
                pr.Discontinued = false;
                //Update(pr);
                ProductElastic productElastic = new ProductElastic();
                productElastic.ProductId = pr.ProductId;
                productElastic.ProductName = pr.ProductName;
                productInESs.Add(productElastic);
                await Console.Out.WriteLineAsync("index " + i);
            }
            //var bulkResponse = _elasticClient.Bulk(b => b
            //       .Index("products")

            //       .CreateMany(productInESs)
            //   );
        }

        public ProductElastic UpdateInElastic(ProductElastic productElastic)
        {
            var response = _elasticClient.Update<ProductElastic, object>(productElastic.ProductId, u => u
          .Index("products") // replace with your actual index name
          .Doc(productElastic)
      //.RetryOnConflict(3) // optional: specify the number of retries on version conflicts
      );

            if (response.IsValid)
            {
                return productElastic;
            }
            else
            {
                // Handle the case where the update was not successful
                return new ProductElastic();
            }
        }
    }
}
