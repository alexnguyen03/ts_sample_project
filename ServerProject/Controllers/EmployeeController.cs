using Microsoft.AspNetCore.Mvc;
using ServerProject.Models;
using ServerProject.Services;
namespace ServerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService = null;
        public EmployeeController(IEmployeeService employeeService)
        {
            this._employeeService = employeeService;
        }
        [HttpGet]
        [Route("getEmployees")]
        public IActionResult getEmployees([FromQuery] int page = 1, [FromQuery] int pageSize = 3, [FromQuery] string filter = "")
        {
            var data = _employeeService.GetAll(page, pageSize, filter);
            return Ok(data);
        }
        [HttpGet]
        [Route("getEmployee")]
        public IActionResult getEmployeeById(int employeeId)
        {
            var data = _employeeService.GetById(employeeId);
            return Ok(data);
        }
        [HttpPost]
        [Route("createEmployee")]
        public IActionResult createEmployee(Employee employee)
        {
            var data = _employeeService.Create(employee);
            return Ok(data);
        }
        [HttpDelete]
        [Route("deleteEmployee")]
        public IActionResult deleteEmployee(int employeeId)
        {
            var data = _employeeService.Delete(employeeId);
            return Ok(data);
        }
        [HttpPut]
        [Route("updateEmployee")]
        public IActionResult updateEmployee(Employee employee)
        {
            var data = _employeeService.Update(employee);
            return Ok(data);
        }
    }
}
