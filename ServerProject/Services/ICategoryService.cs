using ServerProject.Models;

namespace ServerProject.Services
{
    public interface ICategoryService
    {

        public List<Category> GetAll();
        public Category Create(Category category);
        public Category Update(Category category);
        public Category Delete(int categoryId);
    }
}
