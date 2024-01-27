using Microsoft.AspNetCore.Mvc;
using ServerProject.Models;

namespace ServerProject.Services
{
    public interface IEmployeeService
    {
        public object GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 3, [FromQuery] string filter = "");
        public Employee GetById(int EmployeeId);
        public Employee Create(Employee employee);
        public Employee Update(Employee employee);
        public Employee Delete(int employeeId);
    }
}
