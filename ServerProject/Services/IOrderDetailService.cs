using ServerProject.Models;
namespace ServerProject.Services
{
    public interface IOrderDetailService
    {
        public List<OrderDetail> GetAll();
        public OrderDetail Create(OrderDetail orderDetail);
        public OrderDetail Update(OrderDetail orderDetail);
        public OrderDetail Delete(int id);
    }
}
