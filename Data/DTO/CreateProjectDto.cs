using System.ComponentModel.DataAnnotations;

namespace sibersProject.Data.DTO;

public class CreateProjectDto
{
  [Required]
  public string Name { get; set; } = null!;

  [Required]
  public int CustomerCompanyId { get; set; }

  [Required]
  public int ContractorCompanyId { get; set; }

  [Required]
  public int ManagerId { get; set; }

  [Required]
  public DateOnly StartDate { get; set; }

  public DateOnly? EndDate { get; set; }

  [Range(1, 5)]
  public int Priority { get; set; } = 1;

  public string Status { get; set; } = "Active";
}
