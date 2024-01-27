using Microsoft.EntityFrameworkCore;
using ServerProject.Models;
namespace ServerProject.Services
{
    public class OrderService : IOrderService
    {
        private readonly MsdemoContext dbContext = null;
        public OrderService(MsdemoContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public List<Order> GetOrders()
        {
            return dbContext.Orders
                .Include(o => o.OrderDetails)
                //.ThenInclude(it => it.Product)
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .ToList();
        }
        public Order GetAllOrdersByOrderId(int orderId)
        {
            return dbContext.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Employee)
                    .FirstOrDefault(o => o.OrderId == orderId);
        }
        public Order Create(Order order)
        {
            Order newOrder = new Order();
            try
            {
                Customer foundCustomer = dbContext.Customers.Find(order.CustomerId);
                Employee foundEmployee = dbContext.Employees.Find(order.EmployeeId);
                if (foundEmployee == null)
                {
                    throw new Exception("Employee not found");
                }
                if (foundCustomer == null)
                {
                    throw new Exception("Customer not found");
                }
                order.Customer = foundCustomer;
                order.Employee = foundEmployee;
                foundCustomer.Orders.Add(order);
                dbContext.Orders.Add(order);
                dbContext.SaveChangesAsync();
                return order;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while adding the order.", ex);
            }
        }
        public Order Update(Order order)
        {
            try
            {
                Order foundOrder = dbContext.Orders.Find(order.OrderId);
                if (foundOrder != null)
                {
                    foundOrder.OrderDate = order.OrderDate;
                    foundOrder.RequiredDate = order.RequiredDate;
                    foundOrder.ShippedDate = order.ShippedDate;
                    foundOrder.ShipVia = order.ShipVia;
                    foundOrder.Freight = order.Freight;
                    foundOrder.ShipName = order.ShipName;
                    foundOrder.ShipAddress = order.ShipAddress;
                    foundOrder.ShipCity = order.ShipCity;
                    foundOrder.ShipRegion = order.ShipRegion;
                    foundOrder.ShipPostalCode = order.ShipPostalCode;
                    foundOrder.ShipCountry = order.ShipCountry;
                    dbContext.Orders.Update(foundOrder);
                    dbContext.SaveChanges();
                    return foundOrder;
                }
                throw new Exception("Order not found !!!");
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while updating the order.", ex);
            }
        }
        public Order Delete(int orderId)
        {
            try
            {
                Order foundOrder = dbContext.Orders.Find(orderId);
                if (foundOrder != null)
                {
                    dbContext.Orders.Remove(foundOrder);
                    dbContext.SaveChanges();
                    return foundOrder;
                }
                throw new Exception("Customer not found !!!");
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while deleting the customer.", ex);
            }
        }
        public List<Order> GetAllOrdersByCustomerId(string CustomerId)
        {
            throw new NotImplementedException();
        }
    }
}
