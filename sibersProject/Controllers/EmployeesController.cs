using Microsoft.AspNetCore.Mvc;
using sibersProject.Data.DTO;
using sibersProject.Services;

namespace sibersProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // GET: api/employees
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _employeeService.GetAllEmployeesAsync();
        return Ok(employees);
    }

    // GET: api/employees/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
            return NotFound(new { message = $"Employee with id {id} not found" });
        return Ok(employee);
    }

    // POST: api/employees
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var created = await _employeeService.CreateEmployeeAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // PUT: api/employees/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _employeeService.UpdateEmployeeAsync(id, dto);
            if (!updated)
                return NotFound(new { message = $"Employee with id {id} not found" });
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // DELETE: api/employees/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _employeeService.DeleteEmployeeAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Employee with id {id} not found" });
        return NoContent();
    }

    // POST: api/employees/{employeeId}/projects/{projectId}
    [HttpPost("{employeeId}/projects/{projectId}")]
    public async Task<IActionResult> AddToProject(int employeeId, int projectId, [FromQuery] string? role = null)
    {
        try
        {
            var result = await _employeeService.AddEmployeeToProjectAsync(projectId, employeeId, role);
            return Ok(new { message = "Employee added to project successfully" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // DELETE: api/employees/{employeeId}/projects/{projectId}
    [HttpDelete("{employeeId}/projects/{projectId}")]
    public async Task<IActionResult> RemoveFromProject(int employeeId, int projectId)
    {
        try
        {
            var result = await _employeeService.RemoveEmployeeFromProjectAsync(projectId, employeeId);
            return Ok(new { message = "Employee removed from project successfully" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // GET: api/employees/by-project/{projectId}
    [HttpGet("by-project/{projectId}")]
    public async Task<IActionResult> GetByProjectId(int projectId)
    {
        try
        {
            var employees = await _employeeService.GetEmployeesByProjectIdAsync(projectId);
            return Ok(employees);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // GET: api/employees/{employeeId}/projects
    [HttpGet("{employeeId}/projects")]
    public async Task<IActionResult> GetProjectsByEmployee(int employeeId)
    {
        try
        {
            var projects = await _employeeService.GetProjectsByEmployeeIdAsync(employeeId);
            return Ok(projects);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // GET: api/employees/search
    [HttpGet("search")]
    public async Task<IActionResult> SearchEmployees([FromQuery] string? query, [FromQuery] int? limit = 20)
    {
        var employees = await _employeeService.SearchEmployeesAsync(query ?? string.Empty, limit);
        return Ok(employees);
    }
}
