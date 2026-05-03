using System;

namespace sibersProject.Data.DTO;

public class EmployeeDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public string Email { get; set; } = null!;
    public int IsActive { get; set; }
    public ICollection<EmployeeProjectDto>? Projects { get; set; }
}

public class EmployeeProjectDto
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = null!;
    public string? Role { get; set; }
}
