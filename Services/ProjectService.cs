using Microsoft.EntityFrameworkCore;
using sibersProject.Data;
using sibersProject.Data.Entities;
using sibersProject.Data.DTO;

namespace sibersProject.Services;

public class ProjectService : IProjectService
{
  private readonly AppDbContext _context;

  public ProjectService(AppDbContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
  {
    var projects = await _context.Projects
        .Select(p => new ProjectDto
        {
          Id = p.Id,
          Name = p.Name,
          CustomerCompanyId = p.CustomerCompanyId,
          ContractorCompanyId = p.ContractorCompanyId,
          ManagerId = p.ManagerId,
          StartDate = p.StartDate,
          EndDate = p.EndDate,
          Priority = p.Priority,
          Status = p.Status,
          CreatedAt = p.CreatedAt
        })
        .ToListAsync();

    return projects;
  }

  public async Task<ProjectDto?> GetProjectByIdAsync(int id)
  {
    var project = await _context.Projects.FindAsync(id);
    if (project == null) return null;

    return new ProjectDto
    {
      Id = project.Id,
      Name = project.Name,
      CustomerCompanyId = project.CustomerCompanyId,
      ContractorCompanyId = project.ContractorCompanyId,
      ManagerId = project.ManagerId,
      StartDate = project.StartDate,
      EndDate = project.EndDate,
      Priority = project.Priority,
      Status = project.Status,
      CreatedAt = project.CreatedAt
    };
  }

  public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto)
  {
    // Проверяем, существуют ли связанные сущности
    var customerExists = await _context.Companies.AnyAsync(c => c.Id == dto.CustomerCompanyId);
    var contractorExists = await _context.Companies.AnyAsync(c => c.Id == dto.ContractorCompanyId);
    var managerExists = await _context.Employees.AnyAsync(e => e.Id == dto.ManagerId);

    if (!customerExists || !contractorExists || !managerExists)
      throw new ArgumentException("Customer, Contractor or Manager not found");

    var project = new Project
    {
      Name = dto.Name,
      CustomerCompanyId = dto.CustomerCompanyId,
      ContractorCompanyId = dto.ContractorCompanyId,
      ManagerId = dto.ManagerId,
      StartDate = dto.StartDate,
      EndDate = dto.EndDate,
      Priority = dto.Priority,
      Status = dto.Status,
      CreatedAt = DateTime.Now
    };

    _context.Projects.Add(project);
    await _context.SaveChangesAsync();

    return new ProjectDto
    {
      Id = project.Id,
      Name = project.Name,
      CustomerCompanyId = project.CustomerCompanyId,
      ContractorCompanyId = project.ContractorCompanyId,
      ManagerId = project.ManagerId,
      StartDate = project.StartDate,
      EndDate = project.EndDate,
      Priority = project.Priority,
      Status = project.Status,
      CreatedAt = project.CreatedAt
    };
  }

  public async Task<bool> UpdateProjectAsync(int id, UpdateProjectDto dto)
  {
    var project = await _context.Projects.FindAsync(id);
    if (project == null) return false;

    // Проверка связей (опционально, если ID могут меняться)
    var customerExists = await _context.Companies.AnyAsync(c => c.Id == dto.CustomerCompanyId);
    var contractorExists = await _context.Companies.AnyAsync(c => c.Id == dto.ContractorCompanyId);
    var managerExists = await _context.Employees.AnyAsync(e => e.Id == dto.ManagerId);

    if (!customerExists || !contractorExists || !managerExists)
      throw new ArgumentException("One of the related entities does not exist");

    project.Name = dto.Name;
    project.CustomerCompanyId = dto.CustomerCompanyId;
    project.ContractorCompanyId = dto.ContractorCompanyId;
    project.ManagerId = dto.ManagerId;
    project.StartDate = dto.StartDate;
    project.EndDate = dto.EndDate;
    project.Priority = dto.Priority;
    project.Status = dto.Status;

    await _context.SaveChangesAsync();
    return true;
  }

  public async Task<bool> DeleteProjectAsync(int id)
  {
    var project = await _context.Projects.FindAsync(id);
    if (project == null) return false;

    _context.Projects.Remove(project);
    await _context.SaveChangesAsync();
    return true;
  }
}
