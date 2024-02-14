using Microsoft.EntityFrameworkCore;

using ServerProject.Models;

namespace ServerProject.Services
{
    public class PosService : IPosService
    {

        private readonly MsdemoContext dbContext = null;
        private readonly IMessageProducer _messagePublisher;
        public PosService(
            IMessageProducer messagePublisher,
            MsdemoContext dbContext)
        {
            this.dbContext = dbContext;
            _messagePublisher = messagePublisher;
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
            _messagePublisher.SendMessage(newOrder, Channel.ORDER.ToString());
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
