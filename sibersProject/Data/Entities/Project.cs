using System;
using System.Collections.Generic;

namespace sibersProject.Data.Entities;

public partial class Project
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int CustomerCompanyId { get; set; }

    public int ContractorCompanyId { get; set; }

    public int ManagerId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int Priority { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Company ContractorCompany { get; set; } = null!;

    public virtual Company CustomerCompany { get; set; } = null!;

    public virtual Employee Manager { get; set; } = null!;

    public virtual ICollection<ProjectDocument> ProjectDocuments { get; set; } = new List<ProjectDocument>();

    public virtual ICollection<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();
}
