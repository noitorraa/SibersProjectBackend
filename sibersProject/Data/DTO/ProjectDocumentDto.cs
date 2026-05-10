namespace sibersProject.Data.DTO;

public class ProjectDocumentDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public DateTime? UploadedAt { get; set; }
}
