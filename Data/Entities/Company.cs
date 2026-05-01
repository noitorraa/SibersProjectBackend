using System;
using System.Collections.Generic;

namespace sibersProject.Data.Entities;

public partial class Company
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Project> ProjectContractorCompanies { get; set; } = new List<Project>();

    public virtual ICollection<Project> ProjectCustomerCompanies { get; set; } = new List<Project>();
}
