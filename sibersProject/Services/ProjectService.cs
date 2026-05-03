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

  public async Task<IEnumerable<ProjectDto>> GetFilteredProjectsAsync(ProjectQueryParameters parameters)
  {
  var query = _context.Projects.AsQueryable();

  // Filtering
  if (parameters.StartDateFrom.HasValue)
      query = query.Where(p => p.StartDate >= parameters.StartDateFrom.Value);

  if (parameters.StartDateTo.HasValue)
      query = query.Where(p => p.StartDate <= parameters.StartDateTo.Value);

  if (parameters.EndDateFrom.HasValue)
      query = query.Where(p => p.EndDate >= parameters.EndDateFrom.Value);

  if (parameters.EndDateTo.HasValue)
      query = query.Where(p => p.EndDate <= parameters.EndDateTo.Value);

  if (parameters.Priority.HasValue)
      query = query.Where(p => p.Priority == parameters.Priority.Value);

  if (!string.IsNullOrEmpty(parameters.Status))
      query = query.Where(p => p.Status == parameters.Status);

  if (parameters.CustomerCompanyId.HasValue)
      query = query.Where(p => p.CustomerCompanyId == parameters.CustomerCompanyId.Value);

  if (parameters.ContractorCompanyId.HasValue)
      query = query.Where(p => p.ContractorCompanyId == parameters.ContractorCompanyId.Value);

  if (parameters.ManagerId.HasValue)
      query = query.Where(p => p.ManagerId == parameters.ManagerId.Value);

  // Sorting
  if (!string.IsNullOrEmpty(parameters.SortBy))
  {
      query = parameters.SortBy switch
      {
          "Name" => parameters.Descending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
          "StartDate" => parameters.Descending ? query.OrderByDescending(p => p.StartDate) : query.OrderBy(p => p.StartDate),
          "EndDate" => parameters.Descending ? query.OrderByDescending(p => p.EndDate) : query.OrderBy(p => p.EndDate),
          "Priority" => parameters.Descending ? query.OrderByDescending(p => p.Priority) : query.OrderBy(p => p.Priority),
          "Status" => parameters.Descending ? query.OrderByDescending(p => p.Status) : query.OrderBy(p => p.Status),
          "CreatedAt" => parameters.Descending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
          _ => query.OrderBy(p => p.Id)
      };
  }

  var projects = await query
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
  // Check if related entities exist
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

  // Validate related entities (optional, if IDs can change)
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
