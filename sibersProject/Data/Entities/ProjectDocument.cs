using System;
using System.Collections.Generic;

namespace sibersProject.Data.Entities;

public partial class ProjectDocument
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public DateTime? UploadedAt { get; set; }

    public virtual Project Project { get; set; } = null!;
}
