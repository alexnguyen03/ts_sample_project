using Microsoft.EntityFrameworkCore;
using ServerProject.Models;
namespace ServerProject.Services
{
    public class TerritoryService : ITerritoryService
    {
        private readonly MsdemoContext dbContext = null;
        public TerritoryService(MsdemoContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public List<Territory> GetAll()
        {
            return dbContext.Territories.Include(t => t.Region).ToList();
        }
    }
}
