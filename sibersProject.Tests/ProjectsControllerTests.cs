using Microsoft.AspNetCore.Mvc;
using Moq;
using sibersProject.Controllers;
using sibersProject.Data.DTO;
using sibersProject.Services;
using FluentAssertions;
using Xunit;

namespace sibersProject.Tests;

public class ProjectsControllerTests
{
  private readonly Mock<IProjectService> _serviceMock;
  private readonly ProjectsController _controller;

  public ProjectsControllerTests()
  {
    _serviceMock = new Mock<IProjectService>();
    _controller = new ProjectsController(_serviceMock.Object);
  }

  // GET /api/projects (GetAll)
  [Fact]
  public async Task GetAll_ShouldReturnOk_WithListOfProjects()
  {
    // Arrange
    var parameters = new ProjectQueryParameters();
    var expectedProjects = new List<ProjectDto>
      {
          new ProjectDto { Id = 1, Name = "Project1", StartDate = DateOnly.Parse("2025-01-01") },
          new ProjectDto { Id = 2, Name = "Project2", StartDate = DateOnly.Parse("2025-02-01") }
      };
    _serviceMock.Setup(s => s.GetFilteredProjectsAsync(parameters))
                .ReturnsAsync(expectedProjects);

    // Act
    var result = await _controller.GetAll(parameters);

    // Assert
    var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
    var returnedProjects = okResult.Value.Should().BeAssignableTo<IEnumerable<ProjectDto>>().Subject;
    returnedProjects.Should().BeEquivalentTo(expectedProjects);
  }

  // GET /api/projects/{id} (GetById)
  [Fact]
  public async Task GetById_WhenProjectExists_ShouldReturnOk()
  {
    // Arrange
    var project = new ProjectDto { Id = 1, Name = "Test" };
    _serviceMock.Setup(s => s.GetProjectByIdAsync(1))
                .ReturnsAsync(project);

    // Act
    var result = await _controller.GetById(1);

    // Assert
    var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
    okResult.Value.Should().BeEquivalentTo(project);
  }

  [Fact]
  public async Task GetById_WhenProjectDoesNotExist_ShouldReturnNotFound()
  {
    // Arrange
    _serviceMock.Setup(s => s.GetProjectByIdAsync(99))
                .ReturnsAsync((ProjectDto?)null);

    // Act
    var result = await _controller.GetById(99);

    // Assert
    result.Should().BeOfType<NotFoundObjectResult>();
  }

  // POST /api/projects (Create)
  [Fact]
  public async Task Create_WithValidDto_ShouldReturnCreatedAtAction()
  {
    // Arrange
    var createDto = new CreateProjectDto
    {
      Name = "New Project",
      StartDate = DateOnly.Parse("2025-03-20")
    };
    var createdProject = new ProjectDto { Id = 5, Name = "New Project", StartDate = DateOnly.Parse("2025-03-20") };

    _serviceMock.Setup(s => s.CreateProjectAsync(createDto))
                .ReturnsAsync(createdProject);

    // Act
    var result = await _controller.Create(createDto);

    // Assert
    var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
    createdResult.ActionName.Should().Be(nameof(ProjectsController.GetById));
    createdResult.RouteValues["id"].Should().Be(5);
    createdResult.Value.Should().BeEquivalentTo(createdProject);
  }

  [Fact]
  public async Task Create_WhenServiceThrowsArgumentException_ShouldReturnBadRequest()
  {
    // Arrange
    var createDto = new CreateProjectDto { Name = "Invalid" };
    var errorMessage = "Customer not found";
    _serviceMock.Setup(s => s.CreateProjectAsync(createDto))
                .ThrowsAsync(new ArgumentException(errorMessage));

    // Act
    var result = await _controller.Create(createDto);

    // Assert
    var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
    var value = badRequestResult.Value;
    value.Should().NotBeNull();
    var property = value!.GetType().GetProperty("error");
    property.Should().NotBeNull();
    property!.GetValue(value).Should().Be(errorMessage);
  }

  // PUT /api/projects/{id} (Update)
  [Fact]
  public async Task Update_WhenSuccessful_ShouldReturnNoContent()
  {
    // Arrange
    var updateDto = new UpdateProjectDto { Name = "Updated", StartDate = DateOnly.Parse("2025-01-01") };
    _serviceMock.Setup(s => s.UpdateProjectAsync(1, updateDto))
                .ReturnsAsync(true);

    // Act
    var result = await _controller.Update(1, updateDto);

    // Assert
    result.Should().BeOfType<NoContentResult>();
  }

  [Fact]
  public async Task Update_WhenProjectNotFound_ShouldReturnNotFound()
  {
    // Arrange
    var updateDto = new UpdateProjectDto { Name = "Updated", StartDate = DateOnly.Parse("2025-01-01") };
    _serviceMock.Setup(s => s.UpdateProjectAsync(99, updateDto))
                .ReturnsAsync(false);

    // Act
    var result = await _controller.Update(99, updateDto);

    // Assert
    result.Should().BeOfType<NotFoundObjectResult>();
  }

  // DELETE /api/projects/{id}
  [Fact]
  public async Task Delete_WhenExists_ShouldReturnNoContent()
  {
    // Arrange
    _serviceMock.Setup(s => s.DeleteProjectAsync(1))
                .ReturnsAsync(true);

    // Act
    var result = await _controller.Delete(1);

    // Assert
    result.Should().BeOfType<NoContentResult>();
  }

  [Fact]
  public async Task Delete_WhenNotExists_ShouldReturnNotFound()
  {
    // Arrange
    _serviceMock.Setup(s => s.DeleteProjectAsync(99))
                .ReturnsAsync(false);

    // Act
    var result = await _controller.Delete(99);

    // Assert
    result.Should().BeOfType<NotFoundObjectResult>();
  }
}
