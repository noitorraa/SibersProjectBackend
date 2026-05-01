using Microsoft.AspNetCore.Mvc;
using sibersProject.Data.DTO;
using sibersProject.Services;

namespace sibersProject.Controllers;

[ApiController]
[Route("api/[controller]")]   // URL: /api/projects
public class ProjectsController : ControllerBase
{
  private readonly IProjectService _projectService;

  public ProjectsController(IProjectService projectService)
  {
    _projectService = projectService;
  }

  // GET: api/projects
  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    var projects = await _projectService.GetAllProjectsAsync();
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

      return NoContent(); // 204 успех без тела
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
}
