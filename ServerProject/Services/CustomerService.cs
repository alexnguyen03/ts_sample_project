using ClientProject.Model;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ServerProject.Models;

namespace ServerProject.Services
{
    public class CustomerService : ICustomerService
    {

        private readonly MsdemoContext dbContext = null;

        public CustomerService(MsdemoContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Customer Create(Customer customer)
        {
            try
            {
                using (var context = new MsdemoContext())
                {
                    context.Customers.Add(customer);
                    context.SaveChanges();
                }
                return customer;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while adding the customer.", ex);
            }
        }

        public Customer Delete(string customerId)
        {
            try
            {

                using (var context = new MsdemoContext())
                {

                    Customer foundCustomer = context.Customers.Find(customerId);
                    if (foundCustomer != null)
                    {
                        context.Customers.Remove(foundCustomer);
                        context.SaveChanges();
                        return foundCustomer;
                    }
                    throw new Exception("Customer not found !!!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while deleting the customer.", ex);
            }
        }

        public MetaData<Customer> GetAll(RequestParameters customerParameters, [FromQuery] string filter = "")
        {

            var query = dbContext.Customers
                                    .Include(c => c.Orders)
                                    .AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(cust => cust.ContactName.Contains(filter) || cust.City.Contains(filter));
            }


            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / customerParameters.PageSize);

            query = query.Skip((customerParameters.PageNumber - 1) * customerParameters.PageSize).Take(customerParameters.PageSize);
            MetaData<Customer> response = new MetaData<Customer>();
            response.TotalCount = totalCount;
            response.CurrentPage = customerParameters.PageNumber;
            response.TotalPages = totalPages;
            response.PageSize = customerParameters.PageSize;
            List<Customer> customers = query.ToList();
            response.Data = customers;
            return response;
        }

        public Customer GetById(string customerId)
        {
            return dbContext.Customers.Find(customerId);
        }

        public List<Customer> GetCustomersWithoutPage()
        {
            return dbContext.Customers.ToList();
        }

        public Customer Update(Customer customer)
        {
            try
            {

                using (var context = new MsdemoContext())
                {
                    Customer foundCustomer = context.Customers.Find(customer.CustomerId);

                    if (foundCustomer != null)
                    {
                        foundCustomer.CompanyName = customer.CompanyName;
                        foundCustomer.ContactName = customer.ContactName;
                        foundCustomer.ContactTitle = customer.ContactTitle;
                        foundCustomer.Country = customer.Country;
                        foundCustomer.Phone = customer.Phone;
                        foundCustomer.PostalCode = customer.PostalCode;
                        foundCustomer.Country = customer.Country;
                        foundCustomer.Region = customer.Region;
                        foundCustomer.Fax = customer.Fax;
                        foundCustomer.Address = customer.Address;
                        List<CustomerDemographic> customerTypes =
                            (List<CustomerDemographic>)foundCustomer.CustomerTypes;
                        if (customer.CustomerTypes.Count > 0)
                        {
                            customerTypes.AddRange(customer.CustomerTypes);
                        }
                        context.Customers.Update(foundCustomer);

                        context.SaveChanges();
                        return foundCustomer;
                    }
                    throw new Exception("Customer not found !!!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while deleting the customer.", ex);
            }
        }
    }
}
