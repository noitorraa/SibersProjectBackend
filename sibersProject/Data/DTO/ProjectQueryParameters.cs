namespace sibersProject.Data.DTO;

public class ProjectQueryParameters
{
    public DateOnly? StartDateFrom { get; set; }
    public DateOnly? StartDateTo { get; set; }
    public DateOnly? EndDateFrom { get; set; }
    public DateOnly? EndDateTo { get; set; }
    public int? Priority { get; set; }
    public string? Status { get; set; }
    public int? CustomerCompanyId { get; set; }
    public int? ContractorCompanyId { get; set; }
    public int? ManagerId { get; set; }
    // Available sort fields: Name, StartDate, EndDate, Priority, Status, CreatedAt
    public string? SortBy { get; set; }
    public bool Descending { get; set; } = false;
}
