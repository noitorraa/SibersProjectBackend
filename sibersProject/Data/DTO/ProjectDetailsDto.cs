namespace sibersProject.Data.DTO;

public class ProjectDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    
    // Company information
    public int CustomerCompanyId { get; set; }
    public string? CustomerCompanyName { get; set; }
    
    public int ContractorCompanyId { get; set; }
    public string? ContractorCompanyName { get; set; }
    
    // Manager information
    public int ManagerId { get; set; }
    public EmployeeDto? Manager { get; set; }
    
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int Priority { get; set; }
    public string Status { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
}
