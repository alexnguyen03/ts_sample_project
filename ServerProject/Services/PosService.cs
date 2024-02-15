using Microsoft.EntityFrameworkCore;

using ServerProject.Models;

namespace ServerProject.Services
{
    public class PosService : IPosService
    {

        private readonly MsdemoContext dbContext;
        private readonly RabbitMQManager rabbitMQManager;

        public PosService(
            RabbitMQManager rabbitMQManager,
            MsdemoContext dbContext)
        {
            this.dbContext = dbContext;
            this.rabbitMQManager = rabbitMQManager;
        }

        public Pos Create(Pos pos)
        {
            pos.CreateAt = DateTime.Now;
            dbContext.Pos.Add(pos);
            dbContext.SaveChanges();
            Order newOrder = new Order();
            newOrder.CustomerId = pos.CustomerId;
            newOrder.OrderDate = pos.CreateAt;
            newOrder.EmployeeId = pos.EmployeeId;
            List<OrderDetail> listOrderDetail = new List<OrderDetail>();
            foreach (var item in pos.PosDetails!)
            {

                var orderDetail = new OrderDetail();
                orderDetail.ProductId = (int)item.ProductId!;
                orderDetail.UnitPrice = (decimal)item.PricePerUnit!;
                orderDetail.Quantity = (short)item.Quantity!;

                listOrderDetail.Add(orderDetail);
            }
            newOrder.OrderDetails = listOrderDetail;

            rabbitMQManager.SendMessage(newOrder, Channels.ORDER.ToString(), "ORDER_ROUTING");
            return pos;

        }

        public Pos Delete(int posId)
        {
            throw new NotImplementedException();
        }

        public List<Pos> GetAll()
        {
            return dbContext.Pos.Include(p => p.Customer).Include(p => p.PosDetails).ToList();
        }

        public Pos Update(Pos pos)
        {
            throw new NotImplementedException();
        }
    }
}
