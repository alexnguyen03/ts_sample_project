using ServerProject.Models;
namespace ServerProject.Services
{
    public interface ISupplierService
    {
        List<Supplier> GetAll();
        Supplier Create(Supplier supplier);
        Supplier Update(Supplier supplier);
        Supplier Delete(int supplierId);
    }
}
