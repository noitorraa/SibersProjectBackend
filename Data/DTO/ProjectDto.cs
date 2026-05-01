namespace sibersProject.Data.DTO;

public class ProjectDto
{
  public int Id { get; set; }
  public string Name { get; set; } = null!;
  public int CustomerCompanyId { get; set; }
  public int ContractorCompanyId { get; set; }
  public int ManagerId { get; set; }
  public DateOnly StartDate { get; set; }
  public DateOnly? EndDate { get; set; }
  public int Priority { get; set; }
  public string Status { get; set; } = null!;
  public DateTime? CreatedAt { get; set; }
}
