using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ServerProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerProject.Services
{
    public class EmployeeService : IEmployeeService
    {

        private readonly MsdemoContext dbContext = null;

        public EmployeeService(MsdemoContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Employee Create(Employee employee)
        {
            try
            {
                dbContext.Employees.Add(employee);
                dbContext.SaveChanges();
                return employee;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while adding the employee.", ex);
            }

        }

        public Employee Delete(int EmployeeId)
        {
            try
            {
                Employee foundEmployee = dbContext.Employees.Find(EmployeeId);
                if (foundEmployee != null)
                {
                    dbContext.Employees.Remove(foundEmployee);
                    dbContext.SaveChanges();
                    return foundEmployee;
                }
                throw new Exception("Employee not found !!!");
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while deleting the Employee.", ex);
            }
        }

        public object GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 3, [FromQuery] string filter = "")
        {
          
            var query = dbContext.Employees
                                 //.Include(e => e.Orders)
                                 .AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(emp => emp.FirstName.Contains(filter) || emp.Address.Contains(filter));
            }

            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var result = new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize,
                Employees = query.ToList()
            };

            return result ;
        }

        public Employee GetById(int EmployeeId)
        {
            return dbContext.Employees
            .Include(o => o.Orders)
            .FirstOrDefault(o => o.EmployeeId == EmployeeId);
        }

        public Employee Update(Employee employee)
        {
            try
            {

                Employee foundEmployee = dbContext.Employees.Find(employee.EmployeeId);

                if (foundEmployee == null)
                {
                    throw new Exception("Employee not found !!!");
                }
                foundEmployee.LastName = employee.LastName;
                foundEmployee.FirstName = employee.FirstName;
                foundEmployee.Title = employee.Title;
                foundEmployee.TitleOfCourtesy = employee.TitleOfCourtesy;
                foundEmployee.BirthDate = employee.BirthDate;
                foundEmployee.Address = employee.Address;
                foundEmployee.City = employee.City;
                foundEmployee.Region = employee.Region;
                foundEmployee.PostalCode = employee.PostalCode;
                foundEmployee.Country = employee.Country;
                foundEmployee.HomePhone = employee.HomePhone;
                foundEmployee.Extension = employee.Extension;
                foundEmployee.Photo = employee.Photo;
                foundEmployee.Notes = employee.Notes;
                foundEmployee.ReportsTo = employee.ReportsTo;
                foundEmployee.PhotoPath = employee.PhotoPath;
                dbContext.Employees.Update(foundEmployee);
                dbContext.SaveChanges();
                return foundEmployee;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while updating the employee.", ex);
            }
        }
    }
}
