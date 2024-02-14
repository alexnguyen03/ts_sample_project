using ServerProject.Models;
namespace ServerProject.Services
{
    public interface IPosService
    {
        public Pos Create(Pos pos);
        public Pos Delete(int posId);
        public Pos Update(Pos pos);
        public List<Pos> GetAll();
    }
}
