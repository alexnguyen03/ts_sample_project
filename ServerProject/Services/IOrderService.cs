using ServerProject.Models;

namespace ServerProject.Services
{
    public interface IOrderService
    {
        List<Order> GetOrders();
        public Order GetAllOrdersByOrderId(int orderId);
        public Order Create(Order order);
        public Order Update(Order order);
        public Order Delete(int orderId);

    }
}
