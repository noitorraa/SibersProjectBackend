using Microsoft.AspNetCore.Mvc;
using sibersProject.Data.DTO;
using sibersProject.Services;

namespace sibersProject.Controllers;

[ApiController]
[Route("api/[controller]")] // URL: /api/projects
public class ProjectsController : ControllerBase
{
  private readonly IProjectService _projectService;
  private readonly string _documentsPath;
  private readonly IConfiguration _configuration;

  public ProjectsController(IProjectService projectService, IConfiguration configuration)
  {
    _projectService = projectService;
    _configuration = configuration;
    _documentsPath = configuration.GetValue<string>("DocumentsPath") ?? "./Documents";
  }

  // GET: api/projects
  [HttpGet]
  public async Task<IActionResult> GetAll([FromQuery] ProjectQueryParameters parameters)
  {
  var projects = await _projectService.GetFilteredProjectsAsync(parameters);
  return Ok(projects);
  }

  // GET: api/projects/{id}
  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(int id)
  {
  var project = await _projectService.GetProjectByIdAsync(id);
  if (project == null)
    return NotFound(new { message = $"Project with id {id} not found" });

  return Ok(project);
  }

  // POST: api/projects
  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
  {
  if (!ModelState.IsValid)
    return BadRequest(ModelState);

  try
  {
    var created = await _projectService.CreateProjectAsync(dto);
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
  }
  catch (ArgumentException ex)
  {
    return BadRequest(new { error = ex.Message });
  }
  }

  // PUT: api/projects/{id}
  [HttpPut("{id}")]
  public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectDto dto)
  {
  if (!ModelState.IsValid)
    return BadRequest(ModelState);

  try
  {
    var updated = await _projectService.UpdateProjectAsync(id, dto);
    if (!updated)
      return NotFound(new { message = $"Project with id {id} not found" });

    return NoContent(); // 204 No Content
  }
  catch (ArgumentException ex)
  {
    return BadRequest(new { error = ex.Message });
  }
  }

  // DELETE: api/projects/{id}
  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(int id)
  {
  var deleted = await _projectService.DeleteProjectAsync(id);
  if (!deleted)
    return NotFound(new { message = $"Project with id {id} not found" });

  return NoContent();
  }

  // POST: api/projects/{projectId}/documents
  [HttpPost("{projectId}/documents")]
  public async Task<IActionResult> UploadDocument(int projectId, IFormFile file)
  {
    if (file == null || file.Length == 0)
      return BadRequest(new { message = "No file uploaded" });

    try
    {
      // Проверяем существование проекта
      var project = await _projectService.GetProjectByIdAsync(projectId);
      if (project == null)
        return NotFound(new { message = $"Project with id {projectId} not found" });

      // Создаем уникальное имя файла
      var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
      var filePath = $"/{projectId}/{uniqueFileName}";
      
      // Создаем директорию для проекта если не существует
      var projectPath = Path.Combine(_documentsPath, projectId.ToString());
      if (!Directory.Exists(projectPath))
      {
        Directory.CreateDirectory(projectPath);
      }

      // Сохраняем файл
      var fullPath = Path.Combine(projectPath, uniqueFileName);
      using (var stream = new FileStream(fullPath, FileMode.Create))
      {
        await file.CopyToAsync(stream);
      }

      // Сохраняем информацию о документе в БД
      var document = await _projectService.UploadDocumentAsync(projectId, file.FileName, filePath);
      
      return CreatedAtAction(nameof(GetDocumentById), new { projectId, documentId = document.Id }, document);
    }
    catch (ArgumentException ex)
    {
      return BadRequest(new { error = ex.Message });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
    }
  }

  // GET: api/projects/{projectId}/documents/{documentId}
  [HttpGet("{projectId}/documents/{documentId}")]
  public async Task<IActionResult> GetDocumentById(int projectId, int documentId)
  {
    var documents = await _projectService.GetProjectDocumentsAsync(projectId);
    var document = documents.FirstOrDefault(d => d.Id == documentId);
    
    if (document == null)
      return NotFound(new { message = $"Document with id {documentId} not found for project {projectId}" });

    return Ok(document);
  }

  // GET: api/projects/{projectId}/documents
  [HttpGet("{projectId}/documents")]
  public async Task<IActionResult> GetProjectDocuments(int projectId)
  {
    try
    {
      // Проверяем существование проекта
      var project = await _projectService.GetProjectByIdAsync(projectId);
      if (project == null)
        return NotFound(new { message = $"Project with id {projectId} not found" });

      var documents = await _projectService.GetProjectDocumentsAsync(projectId);
      return Ok(documents);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
    }
  }

  // DELETE: api/projects/{projectId}/documents/{documentId}
  [HttpDelete("{projectId}/documents/{documentId}")]
  public async Task<IActionResult> DeleteDocument(int projectId, int documentId)
  {
    try
    {
      var deleted = await _projectService.DeleteDocumentAsync(projectId, documentId);
      if (!deleted)
        return NotFound(new { message = $"Document with id {documentId} not found for project {projectId}" });

      return NoContent();
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
    }
  }
}
