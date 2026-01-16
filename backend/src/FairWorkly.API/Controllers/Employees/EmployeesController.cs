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
        
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { 
                status = "healthy", 
                timestamp = DateTime.UtcNow,
                message = "Backend is running!"
            });
        }
    }
}