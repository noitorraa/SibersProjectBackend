using Microsoft.EntityFrameworkCore;
using sibersProject.Data;
using sibersProject.Data.Entities;
using sibersProject.Data.DTO;
using System.Linq.Expressions;

namespace sibersProject.Services;

public class ProjectService : IProjectService
{
  private readonly AppDbContext _context;
  private readonly string _documentsPath;

  public ProjectService(AppDbContext context, IConfiguration configuration)
  {
    _context = context;
    _documentsPath = configuration.GetValue<string>("DocumentsPath") ?? "./Documents";
    
    if (!Directory.Exists(_documentsPath))
    {
      Directory.CreateDirectory(_documentsPath);
    }
  }

  private static Expression<Func<Project, ProjectDetailsDto>> ProjectToDetailsDto => p => new ProjectDetailsDto
  {
    Id = p.Id,
    Name = p.Name,
    CustomerCompanyId = p.CustomerCompanyId,
    CustomerCompanyName = p.CustomerCompany.Name,
    ContractorCompanyId = p.ContractorCompanyId,
    ContractorCompanyName = p.ContractorCompany.Name,
    ManagerId = p.ManagerId,
    Manager = new EmployeeDto
    {
      Id = p.Manager.Id,
      FirstName = p.Manager.FirstName,
      LastName = p.Manager.LastName,
      MiddleName = p.Manager.MiddleName,
      Email = p.Manager.Email,
      IsActive = p.Manager.IsActive
    },
    StartDate = p.StartDate,
    EndDate = p.EndDate,
    Priority = p.Priority,
    Status = p.Status ?? string.Empty,
    CreatedAt = p.CreatedAt
  };

  public async Task<IEnumerable<ProjectDetailsDto>> GetAllProjectsAsync()
  {
    return await _context.Projects
        .Include(p => p.CustomerCompany)
        .Include(p => p.ContractorCompany)
        .Include(p => p.Manager)
        .Select(ProjectToDetailsDto)
        .ToListAsync();
  }

  public async Task<IEnumerable<ProjectDetailsDto>> GetFilteredProjectsAsync(ProjectQueryParameters parameters)
  {
    var query = _context.Projects
        .Include(p => p.CustomerCompany)
        .Include(p => p.ContractorCompany)
        .Include(p => p.Manager)
        .AsQueryable();

    query = ApplyFilters(query, parameters);
    query = ApplySorting(query, parameters);

    return await query.Select(ProjectToDetailsDto).ToListAsync();
  }

  public async Task<ProjectDetailsDto?> GetProjectByIdAsync(int id)
  {
    return await _context.Projects
        .Include(p => p.CustomerCompany)
        .Include(p => p.ContractorCompany)
        .Include(p => p.Manager)
        .Where(p => p.Id == id)
        .Select(ProjectToDetailsDto)
        .FirstOrDefaultAsync();
  }

  public async Task<ProjectDetailsDto> CreateProjectAsync(CreateProjectDto dto)
  {
    if (!await ValidateRelatedEntitiesAsync(dto.CustomerCompanyId, dto.ContractorCompanyId, dto.ManagerId))
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
      CreatedAt = DateTime.UtcNow
    };

    _context.Projects.Add(project);
    await _context.SaveChangesAsync();

    return await GetProjectByIdAsync(project.Id) 
      ?? throw new InvalidOperationException("Failed to retrieve created project");
  }

  public async Task<bool> UpdateProjectAsync(int id, UpdateProjectDto dto)
  {
    var project = await _context.Projects.FindAsync(id);
    if (project == null) return false;

    if (!await ValidateRelatedEntitiesAsync(dto.CustomerCompanyId, dto.ContractorCompanyId, dto.ManagerId))
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

  public async Task<ProjectDocumentDto> UploadDocumentAsync(int projectId, string fileName, string filePath)
  {
    var project = await _context.Projects.FindAsync(projectId);
    if (project == null)
      throw new ArgumentException($"Project with id {projectId} not found");

    var document = new ProjectDocument
    {
      ProjectId = projectId,
      FileName = fileName,
      FilePath = filePath,
      UploadedAt = DateTime.UtcNow
    };

    _context.ProjectDocuments.Add(document);
    await _context.SaveChangesAsync();

    return new ProjectDocumentDto
    {
      Id = document.Id,
      ProjectId = document.ProjectId,
      FileName = document.FileName,
      FilePath = document.FilePath,
      UploadedAt = document.UploadedAt
    };
  }

  public async Task<bool> DeleteDocumentAsync(int projectId, int documentId)
  {
    var document = await _context.ProjectDocuments
      .FirstOrDefaultAsync(d => d.Id == documentId && d.ProjectId == projectId);
    
    if (document == null) return false;

    // Удаляем физический файл, если он существует
    var fullPath = Path.Combine(_documentsPath, document.FilePath.TrimStart('/'));
    if (File.Exists(fullPath))
    {
      File.Delete(fullPath);
    }

    _context.ProjectDocuments.Remove(document);
    await _context.SaveChangesAsync();
    return true;
  }

  public async Task<IEnumerable<ProjectDocumentDto>> GetProjectDocumentsAsync(int projectId)
  {
    var documents = await _context.ProjectDocuments
      .Where(d => d.ProjectId == projectId)
      .Select(d => new ProjectDocumentDto
      {
        Id = d.Id,
        ProjectId = d.ProjectId,
        FileName = d.FileName,
        FilePath = d.FilePath,
        UploadedAt = d.UploadedAt
      })
      .ToListAsync();

    return documents;
  }

  private IQueryable<Project> ApplyFilters(IQueryable<Project> query, ProjectQueryParameters parameters)
  {
    if (parameters.StartDateFrom.HasValue)
      query = query.Where(p => p.StartDate >= parameters.StartDateFrom.Value);

    if (parameters.StartDateTo.HasValue)
      query = query.Where(p => p.StartDate <= parameters.StartDateTo.Value);

    if (parameters.EndDateFrom.HasValue)
      query = query.Where(p => p.EndDate.HasValue && p.EndDate.Value >= parameters.EndDateFrom.Value);

    if (parameters.EndDateTo.HasValue)
      query = query.Where(p => p.EndDate.HasValue && p.EndDate.Value <= parameters.EndDateTo.Value);

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

    return query;
  }

  private IQueryable<Project> ApplySorting(IQueryable<Project> query, ProjectQueryParameters parameters)
  {
    if (string.IsNullOrEmpty(parameters.SortBy))
      return query.OrderBy(p => p.Id);

    return parameters.SortBy.ToLower() switch
    {
      "name" => parameters.Descending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
      "startdate" => parameters.Descending ? query.OrderByDescending(p => p.StartDate) : query.OrderBy(p => p.StartDate),
      "enddate" => parameters.Descending ? query.OrderByDescending(p => p.EndDate) : query.OrderBy(p => p.EndDate),
      "priority" => parameters.Descending ? query.OrderByDescending(p => p.Priority) : query.OrderBy(p => p.Priority),
      "status" => parameters.Descending ? query.OrderByDescending(p => p.Status) : query.OrderBy(p => p.Status),
      "createdat" => parameters.Descending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
      _ => query.OrderBy(p => p.Id)
    };
  }

  private async Task<bool> ValidateRelatedEntitiesAsync(int customerCompanyId, int contractorCompanyId, int managerId)
  {
    var customerExists = await _context.Companies.AnyAsync(c => c.Id == customerCompanyId);
    var contractorExists = await _context.Companies.AnyAsync(c => c.Id == contractorCompanyId);
    var managerExists = await _context.Employees.AnyAsync(e => e.Id == managerId);

    return customerExists && contractorExists && managerExists;
  }
}
