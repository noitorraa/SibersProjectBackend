using sibersProject.Data.DTO;

namespace sibersProject.Services;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
    Task<EmployeeDto?> GetEmployeeByIdAsync(int id);
    Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto);
    Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto);
    Task<bool> DeleteEmployeeAsync(int id);

    // Methods for managing Project-Employee relationships
    Task<bool> AddEmployeeToProjectAsync(int projectId, int employeeId, string? role = null);
    Task<bool> RemoveEmployeeFromProjectAsync(int projectId, int employeeId);
    Task<IEnumerable<EmployeeDto>> GetEmployeesByProjectIdAsync(int projectId);
    Task<IEnumerable<ProjectDto>> GetProjectsByEmployeeIdAsync(int employeeId);
}
