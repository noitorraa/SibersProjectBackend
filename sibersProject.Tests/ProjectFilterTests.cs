using Microsoft.AspNetCore.Mvc;
using Moq;
using sibersProject.Controllers;
using sibersProject.Data.DTO;
using sibersProject.Services;
using FluentAssertions;
using Xunit;

namespace sibersProject.Tests;

public class ProjectFilterTests
{
    private readonly Mock<IProjectService> _serviceMock;
    private readonly ProjectsController _controller;

    public ProjectFilterTests()
    {
        _serviceMock = new Mock<IProjectService>();
        _controller = new ProjectsController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_WithDateRangeFilter_ShouldReturnFilteredProjects()
    {
        // Arrange
        var parameters = new ProjectQueryParameters
        {
            StartDateFrom = DateOnly.Parse("2025-01-01"),
            StartDateTo = DateOnly.Parse("2025-12-31")
        };
        var expectedProjects = new List<ProjectDto>
        {
            new ProjectDto { Id = 1, Name = "Project1", StartDate = DateOnly.Parse("2025-06-01") },
            new ProjectDto { Id = 2, Name = "Project2", StartDate = DateOnly.Parse("2025-03-15") }
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

    [Fact]
    public async Task GetAll_WithPriorityFilter_ShouldReturnFilteredProjects()
    {
        // Arrange
        var parameters = new ProjectQueryParameters { Priority = 1 };
        var expectedProjects = new List<ProjectDto>
        {
            new ProjectDto { Id = 1, Name = "High Priority", Priority = 1 },
            new ProjectDto { Id = 3, Name = "Urgent", Priority = 1 }
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

    [Fact]
    public async Task GetAll_WithStatusFilter_ShouldReturnFilteredProjects()
    {
        // Arrange
        var parameters = new ProjectQueryParameters { Status = "Active" };
        var expectedProjects = new List<ProjectDto>
        {
            new ProjectDto { Id = 1, Name = "Active Project", Status = "Active" }
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

    [Fact]
    public async Task GetAll_WithSortByNameAscending_ShouldReturnSortedProjects()
    {
        // Arrange
        var parameters = new ProjectQueryParameters { SortBy = "Name", Descending = false };
        var expectedProjects = new List<ProjectDto>
        {
            new ProjectDto { Id = 2, Name = "Alpha" },
            new ProjectDto { Id = 1, Name = "Beta" },
            new ProjectDto { Id = 3, Name = "Gamma" }
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

    [Fact]
    public async Task GetAll_WithSortByPriorityDescending_ShouldReturnSortedProjects()
    {
        // Arrange
        var parameters = new ProjectQueryParameters { SortBy = "Priority", Descending = true };
        var expectedProjects = new List<ProjectDto>
        {
            new ProjectDto { Id = 1, Name = "Critical", Priority = 3 },
            new ProjectDto { Id = 2, Name = "High", Priority = 2 },
            new ProjectDto { Id = 3, Name = "Low", Priority = 1 }
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

    [Fact]
    public async Task GetAll_WithSortByStartDate_ShouldReturnSortedProjects()
    {
        // Arrange
        var parameters = new ProjectQueryParameters { SortBy = "StartDate", Descending = false };
        var expectedProjects = new List<ProjectDto>
        {
            new ProjectDto { Id = 1, Name = "Early", StartDate = DateOnly.Parse("2025-01-01") },
            new ProjectDto { Id = 2, Name = "Middle", StartDate = DateOnly.Parse("2025-06-01") },
            new ProjectDto { Id = 3, Name = "Late", StartDate = DateOnly.Parse("2025-12-01") }
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

    [Fact]
    public async Task GetAll_WithCombinedFilters_ShouldReturnFilteredProjects()
    {
        // Arrange
        var parameters = new ProjectQueryParameters
        {
            Priority = 1,
            Status = "Active",
            SortBy = "Name",
            Descending = false
        };
        var expectedProjects = new List<ProjectDto>
        {
            new ProjectDto { Id = 1, Name = "Active High Priority", Priority = 1, Status = "Active" }
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

    [Fact]
    public async Task GetAll_WithNoParameters_ShouldReturnAllProjects()
    {
        // Arrange
        var parameters = new ProjectQueryParameters();
        var expectedProjects = new List<ProjectDto>
        {
            new ProjectDto { Id = 1, Name = "Project1" },
            new ProjectDto { Id = 2, Name = "Project2" }
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

    [Fact]
    public async Task GetAll_WithCustomerCompanyFilter_ShouldReturnFilteredProjects()
    {
        // Arrange
        var parameters = new ProjectQueryParameters { CustomerCompanyId = 1 };
        var expectedProjects = new List<ProjectDto>
        {
            new ProjectDto { Id = 1, Name = "Company1 Project", CustomerCompanyId = 1 }
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

    [Fact]
    public async Task GetAll_WithManagerFilter_ShouldReturnFilteredProjects()
    {
        // Arrange
        var parameters = new ProjectQueryParameters { ManagerId = 1 };
        var expectedProjects = new List<ProjectDto>
        {
            new ProjectDto { Id = 1, Name = "Managed by Ivan", ManagerId = 1 }
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
}
