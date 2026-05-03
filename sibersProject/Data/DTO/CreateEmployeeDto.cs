using System.ComponentModel.DataAnnotations;

namespace sibersProject.Data.DTO;

public class CreateEmployeeDto
{
    [Required]
    public string FirstName { get; set; } = null!;
    
    [Required]
    public string LastName { get; set; } = null!;
    
    public string? MiddleName { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    
    public int IsActive { get; set; } = 1;
}
