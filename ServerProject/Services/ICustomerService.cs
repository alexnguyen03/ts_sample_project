using ClientProject.Model;

using Microsoft.AspNetCore.Mvc;

using ServerProject.Models;
namespace ServerProject.Services
{
    public interface ICustomerService
    {
        public MetaData<Customer> GetAll(RequestParameters customerParameters, [FromQuery] string filter = "");
        public List<Customer> GetCustomersWithoutPage();
        public Customer Create(Customer customer);
        public Customer Update(Customer customer);
        public Customer Delete(string customerId);
        public Customer GetById(string customerId);
    }
}
