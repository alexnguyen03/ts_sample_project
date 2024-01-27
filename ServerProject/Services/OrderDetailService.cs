using Microsoft.EntityFrameworkCore;
using ServerProject.Models;
namespace ServerProject.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly MsdemoContext dbContext = null;
        public OrderDetailService(MsdemoContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public OrderDetail Create(OrderDetail orderDetail)
        {
            try
            {
                using (var context = new MsdemoContext())
                {
                    context.OrderDetails.Add(orderDetail);
                    context.SaveChanges();
                }
                return orderDetail;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while adding the order detail.", ex);
            }
        }
        public OrderDetail Delete(int id)
        {
            throw new NotImplementedException();
        }
        public List<OrderDetail> GetAll()
        {
            return dbContext.OrderDetails.Include(od => od.Product).ToList();
        }
        public OrderDetail Update(OrderDetail orderDetail)
        {
            throw new NotImplementedException();
        }
    }
}
