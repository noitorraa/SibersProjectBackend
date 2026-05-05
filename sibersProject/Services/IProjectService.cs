using sibersProject.Data.DTO;

namespace sibersProject.Services;

public interface IProjectService
{
  Task<IEnumerable<ProjectDetailsDto>> GetAllProjectsAsync();
  Task<IEnumerable<ProjectDetailsDto>> GetFilteredProjectsAsync(ProjectQueryParameters parameters);
  Task<ProjectDetailsDto?> GetProjectByIdAsync(int id);
  Task<ProjectDetailsDto> CreateProjectAsync(CreateProjectDto dto);
  Task<bool> UpdateProjectAsync(int id, UpdateProjectDto dto);
  Task<bool> DeleteProjectAsync(int id);
}
