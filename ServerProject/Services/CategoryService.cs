using Microsoft.EntityFrameworkCore;
using ServerProject.Models;

namespace ServerProject.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly MsdemoContext dbContext = null;
        public CategoryService(MsdemoContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public Category Create(Category category)
        {
            try
            {

                dbContext.Categories.Add(category);
                dbContext.SaveChanges();

                return category;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while", ex);
            }

        }
        public Category Delete(int categoryId)
        {
            try
            {

                Category category = dbContext.Categories.Find(categoryId);

                dbContext.Categories.Remove(category);
                dbContext.SaveChanges();

                return category;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while", ex);
            }

        }

        public List<Category> GetAll()
        {
            return dbContext.Categories.Include(c => c.Products).ToList();
        }
        public Category Update(Category category)
        {
            try
            {
                Category foundCategory = dbContext.Categories.Find(category.CategoryId);
                if (foundCategory == null)
                {
                    throw new Exception("Category not found");
                }
                foundCategory.CategoryName = category.CategoryName;
                foundCategory.Description = category.Description;
                foundCategory.Picture = category.Picture;
                dbContext.Categories.Update(foundCategory);
                dbContext.SaveChanges();

                return category;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while", ex);
            }
        }
    }
}
