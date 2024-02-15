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
        public ProductService(
            MsdemoContext dbContext,
         IElasticClient elasticClient)
        {
            this.dbContext = dbContext;
            _elasticClient = elasticClient;
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
                return foundProduct;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while update the product.", ex)!;
            }
        }
        public ProductElastic AddProductToES(string indexName, ProductElastic document)
        {
            // Kiểm tra xem index có tồn tại hay không, nếu không thì tạo mới
            CreateIndexIfNotExists<ProductElastic>(indexName);

            // Index document
            var indexResponse = _elasticClient.Index(document, i => i
                .Index(indexName)
            );

            if (indexResponse.IsValid)
            {
                Console.WriteLine($"Document added successfully. ID: {indexResponse.Id}");
                return document;
            }
            else
            {
                Console.WriteLine($"Error adding document: {indexResponse.OriginalException.Message}");
                return null;
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



        private void CreateIndexIfNotExists<T>(string indexName) where T : class
        {
            if (!_elasticClient.Indices.Exists(indexName).Exists)
            {
                _elasticClient.Indices.Create(indexName, c => c
                    .Map<T>(m => m.AutoMap())
                );
                Console.WriteLine($"Index '{indexName}' created successfully.");
            }
        }

        public ProductElastic UpdateDocumentInES(string indexName, ProductElastic updatedDocument)
        {
            var searchResponse = _elasticClient.Search<ProductElastic>(s => s
                    .Index(indexName)
                    .Query(q => q
                        .Term(t => t
                            .Field(f => f.ProductId) // Thay thế ProductId bằng tên trường chứa productId
                            .Value(updatedDocument.ProductId)
                        )
                    )
                );

            // Kiểm tra xem có document nào được tìm thấy không
            if (searchResponse.IsValid && searchResponse.Hits.Count > 0)
            {
                // Lấy document đầu tiên tìm thấy
                var documentId = searchResponse.Hits.First().Id;

                // Cập nhật document
                var updateResponse = _elasticClient.Update<ProductElastic>(documentId, u => u
                    .Index(indexName)
                    .Doc(updatedDocument)
                );

                if (updateResponse.IsValid)
                {
                    Console.WriteLine($"Document updated successfully. ID: {updateResponse.Id}");
                    return updatedDocument;
                }
                else
                {
                    Console.WriteLine($"Error updating document: {updateResponse.OriginalException.Message}");
                    return new ProductElastic();
                }
            }
            else
            {
                Console.WriteLine($"No document found with productId: {updatedDocument}");
                return new ProductElastic();

            }
        }
    }
}
