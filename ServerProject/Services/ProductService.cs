using ClientProject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ServerProject.Models;
namespace ServerProject.Services
{
    public class ProductService : IProductService
    {
        private readonly MsdemoContext dbContext = null;
        public ProductService(MsdemoContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public Product Create(Product product)
        {
            try
            {
                Category foundCategory = dbContext.Categories.Find(product.CategoryId);
                var foundProductWithSupplier = dbContext.Products
                    .Include(od => od.Supplier)
                    .FirstOrDefault(od => od.ProductId == product.ProductId);
                Supplier supplier = foundProductWithSupplier?.Supplier;
                product.Category = foundCategory;
                product.Supplier = supplier;
                dbContext.Products.Update(product);
                dbContext.SaveChanges();
                return product;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while deleting the customer.", ex);
            }
        }
        public Product Delete(int productId)
        {
            try
            {
                Product foundProduct = dbContext.Products.Find(productId);
                if (foundProduct == null)
                {
                    throw new Exception("Product not found !!!");
                }
                dbContext.Products.Remove(foundProduct);
                dbContext.SaveChanges();
                return foundProduct;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while deleting the product.", ex);
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
                .FirstOrDefault(o => o.ProductId == ProductId);
        }
        public Product Update(Product product)
        {
            try
            {
                Product foundProduct = dbContext.Products.Find(product.ProductId);
                Category foundCategory = dbContext.Categories.Find(product.CategoryId);
                var foundProductWithSupplier = dbContext.Products
                    .Include(od => od.Supplier)
                    .FirstOrDefault(od => od.ProductId == product.ProductId);
                Supplier supplier = foundProductWithSupplier?.Supplier;
                if (foundProduct == null)
                {
                    throw new Exception("Product not found !!!");
                }
                foundProduct.ProductName = product.ProductName;
                foundProduct.SupplierId = product.SupplierId;
                foundProduct.Category = foundCategory;
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
                throw new Exception("Error occurred while deleting the customer.", ex);
            }
        }
    }
}
