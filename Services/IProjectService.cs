using sibersProject.Data.DTO;

namespace sibersProject.Services;

public interface IProjectService
{
  Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
  Task<ProjectDto?> GetProjectByIdAsync(int id);
  Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto);
  Task<bool> UpdateProjectAsync(int id, UpdateProjectDto dto);
  Task<bool> DeleteProjectAsync(int id);
}
