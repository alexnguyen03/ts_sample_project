using Microsoft.EntityFrameworkCore;
using ServerProject.Models;
namespace ServerProject.Services
{
    public class ShipperService : IShipperService
    {
        public readonly MsdemoContext dbContext = null;
        public ShipperService(MsdemoContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public Shipper Create(Shipper shipper)
        {
            try
            {
                dbContext.Shippers.Add(shipper);
                dbContext.SaveChanges();
                Console.WriteLine("shipper Id ",
                    shipper.ShipperId);
                return shipper;
            }
            catch (Exception)
            {
                throw new Exception("Error occurred while adding the Shipper");
            }
        }
        public Shipper Delete(int shipperId)
        {
            try
            {
                Shipper shipper = dbContext.Shippers.Find(shipperId);
                dbContext.Shippers.Remove(shipper);
                dbContext.SaveChanges();
                Console.WriteLine("shipper Id ",
                    shipper.ShipperId);
                return shipper;
            }
            catch (Exception)
            {
                throw new Exception("Error occurred while deleting the Shipper");
            }
        }
        public List<Shipper> GetAll()
        {
            return dbContext.Shippers.Include(s => s.Orders).ToList();
        }
        public Shipper Update(Shipper shipper)
        {
            try
            {
                dbContext.Shippers.Update(shipper);
                dbContext.SaveChanges();
                Console.WriteLine("shipper Id ",
                    shipper.ShipperId);
                return shipper;
            }
            catch (Exception)
            {
                throw new Exception("Error occurred while updating the Shipper");
            }
        }
    }
}
