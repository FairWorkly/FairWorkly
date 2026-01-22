using FairWorkly.Application.Employees.Interfaces;
using FairWorkly.Application.Employees.Services;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.Controllers.Employees
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
    }
}
