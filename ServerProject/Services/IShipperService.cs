using ServerProject.Models;
namespace ServerProject.Services
{
    public interface IShipperService
    {
        List<Shipper> GetAll();
        Shipper Create(Shipper shipper);
        Shipper Update(Shipper shipper);
        Shipper Delete(int shipperId);
    }
}
