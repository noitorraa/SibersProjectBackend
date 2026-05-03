using Microsoft.EntityFrameworkCore;
using sibersProject.Data;
using sibersProject.Data.Entities;
using sibersProject.Data.DTO;

namespace sibersProject.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;

    public EmployeeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
    {
        var employees = await _context.Employees
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                MiddleName = e.MiddleName,
                Email = e.Email,
                IsActive = e.IsActive,
                Projects = e.ProjectEmployees.Select(pe => new EmployeeProjectDto
                {
                    ProjectId = pe.ProjectId,
                    ProjectName = pe.Project.Name,
                    Role = pe.Role
                }).ToList()
            })
            .ToListAsync();

        return employees;
    }

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.ProjectEmployees)
                .ThenInclude(pe => pe.Project)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) return null;

        return new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            MiddleName = employee.MiddleName,
            Email = employee.Email,
            IsActive = employee.IsActive,
            Projects = employee.ProjectEmployees.Select(pe => new EmployeeProjectDto
            {
                ProjectId = pe.ProjectId,
                ProjectName = pe.Project.Name,
                Role = pe.Role
            }).ToList()
        };
    }

    public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto)
    {
        // Проверяем, существует ли email
        var emailExists = await _context.Employees.AnyAsync(e => e.Email == dto.Email);
        if (emailExists)
            throw new ArgumentException("Employee with this email already exists");

        var employee = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            MiddleName = dto.MiddleName,
            Email = dto.Email,
            IsActive = dto.IsActive
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            MiddleName = employee.MiddleName,
            Email = employee.Email,
            IsActive = employee.IsActive,
            Projects = new List<EmployeeProjectDto>()
        };
    }

    public async Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return false;

        // Проверяем email на уникальность (исключая текущего сотрудника)
        var emailExists = await _context.Employees
            .AnyAsync(e => e.Email == dto.Email && e.Id != id);
        if (emailExists)
            throw new ArgumentException("Employee with this email already exists");

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.MiddleName = dto.MiddleName;
        employee.Email = dto.Email;
        employee.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return false;

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddEmployeeToProjectAsync(int projectId, int employeeId, string? role = null)
    {
        var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId);
        var employeeExists = await _context.Employees.AnyAsync(e => e.Id == employeeId);
        
        if (!projectExists || !employeeExists)
            throw new ArgumentException("Project or Employee not found");

        var existingLink = await _context.ProjectEmployees
            .FirstOrDefaultAsync(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId);
        
        if (existingLink != null)
            throw new ArgumentException("Employee is already assigned to this project");

        var projectEmployee = new ProjectEmployee
        {
            ProjectId = projectId,
            EmployeeId = employeeId,
            Role = role
        };

        _context.ProjectEmployees.Add(projectEmployee);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveEmployeeFromProjectAsync(int projectId, int employeeId)
    {
        var projectEmployee = await _context.ProjectEmployees
            .FirstOrDefaultAsync(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId);

        if (projectEmployee == null)
            throw new ArgumentException("Employee is not assigned to this project");

        _context.ProjectEmployees.Remove(projectEmployee);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<EmployeeDto>> GetEmployeesByProjectIdAsync(int projectId)
    {
        var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId);
        if (!projectExists)
            throw new ArgumentException("Project not found");

        var employees = await _context.ProjectEmployees
            .Where(pe => pe.ProjectId == projectId)
            .Include(pe => pe.Employee)
            .Select(pe => new EmployeeDto
            {
                Id = pe.Employee.Id,
                FirstName = pe.Employee.FirstName,
                LastName = pe.Employee.LastName,
                MiddleName = pe.Employee.MiddleName,
                Email = pe.Employee.Email,
                IsActive = pe.Employee.IsActive,
                Projects = pe.Employee.ProjectEmployees.Select(p => new EmployeeProjectDto
                {
                    ProjectId = p.ProjectId,
                    ProjectName = p.Project.Name,
                    Role = p.Role
                }).ToList()
            })
            .ToListAsync();

        return employees;
    }

    public async Task<IEnumerable<ProjectDto>> GetProjectsByEmployeeIdAsync(int employeeId)
    {
        var employeeExists = await _context.Employees.AnyAsync(e => e.Id == employeeId);
        if (!employeeExists)
            throw new ArgumentException("Employee not found");

        var projects = await _context.ProjectEmployees
            .Where(pe => pe.EmployeeId == employeeId)
            .Include(pe => pe.Project)
            .Select(pe => new ProjectDto
            {
                Id = pe.Project.Id,
                Name = pe.Project.Name,
                CustomerCompanyId = pe.Project.CustomerCompanyId,
                ContractorCompanyId = pe.Project.ContractorCompanyId,
                ManagerId = pe.Project.ManagerId,
                StartDate = pe.Project.StartDate,
                EndDate = pe.Project.EndDate,
                Priority = pe.Project.Priority,
                Status = pe.Project.Status,
                CreatedAt = pe.Project.CreatedAt
            })
            .ToListAsync();

        return projects;
    }
}
