using ServerProject.Models;
namespace ServerProject.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly MsdemoContext dbContext = null;
        public SupplierService(MsdemoContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public Supplier Create(Supplier supplier)
        {
            try
            {
                dbContext.Suppliers.Add(supplier);
                dbContext.SaveChangesAsync();
                return supplier;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while adding the supplier.", ex);
            }
        }
        public Supplier Delete(int supplierId)
        {
            try
            {
                Supplier foundSupplier = dbContext.Suppliers.Find(supplierId);
                if (foundSupplier == null)
                {
                    throw new Exception("Supplier not found !!!");
                }
                dbContext.Suppliers.Remove(foundSupplier);
                dbContext.SaveChanges();
                return foundSupplier;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while deleting the customer.", ex);
            }
        }
        public List<Supplier> GetAll()
        {
            return dbContext.Suppliers.ToList();
        }
        public Supplier Update(Supplier supplier)
        {
            try
            {
                Supplier foundSupplier = dbContext.Suppliers.Find(supplier.SupplierId);
                if (foundSupplier == null)
                {
                    throw new Exception("Supplier not found !!!");
                }
                dbContext.Suppliers.Update(foundSupplier);
                dbContext.SaveChanges();
                return foundSupplier;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while deleting the customer.", ex);
            }
        }
    }
}
