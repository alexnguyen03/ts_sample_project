using ClientProject.Model;

using Microsoft.AspNetCore.Mvc;

using ServerProject.Models;
using ServerProject.Services;
namespace ServerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService = null;
        public CustomerController(ICustomerService customerService)
        {
            this._customerService = customerService;
        }
        [HttpGet]
        [Route("getCustomers")]
        public MetaData<Customer> GetCustomers([FromQuery] RequestParameters customerParameters, [FromQuery] string filter = "")
        {
            MetaData<Customer> data = _customerService.GetAll(customerParameters, filter);
            //Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(data));
            return data;
        }
        [HttpGet]
        [Route("getCustomersWithoutPage")]
        public List<Customer> GetCustomersWithoutPage()
        {
            List<Customer> data = _customerService.GetCustomersWithoutPage();
            return data;
        }
        [HttpPost]
        [Route("createCustomer")]
        public IActionResult createCustomers(Customer customer)
        {
            var data = _customerService.Create(customer);
            return Ok(data);
        }
        [HttpPut]
        [Route("updateCustomer")]
        public IActionResult updateCustomers(Customer customer)
        {
            var data = _customerService.Update(customer);
            return Ok(data);
        }
        [HttpDelete]
        [Route("deleteCustomer")]
        public IActionResult deleteCustomers(string customerId)
        {
            var data = _customerService.Delete(customerId);
            return Ok(data);
        }
    }
}
