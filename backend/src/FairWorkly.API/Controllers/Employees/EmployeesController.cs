using FairWorkly.Application.Employees.Services;
using FairWorkly.Domain.Employees.Dtos;
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

        [HttpPost]
        [ProducesResponseType(typeof(CreateEmployeeResponseDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<CreateEmployeeResponseDto>> Create([FromBody] CreateEmployeeRequestDto request)
        {
            var result = await _employeeService.CreateEmployeeAsync(request);
            return StatusCode(StatusCodes.Status201Created, result);
        }
    }
}
