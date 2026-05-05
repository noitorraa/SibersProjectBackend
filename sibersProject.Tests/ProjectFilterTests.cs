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
        var expectedProjects = new List<ProjectDetailsDto>
        {
            new ProjectDetailsDto { Id = 1, Name = "Project1", Manager = new EmployeeDto { FirstName = "John", LastName = "Doe" }, StartDate = DateOnly.Parse("2025-06-01") },
            new ProjectDetailsDto { Id = 2, Name = "Project2", Manager = new EmployeeDto { FirstName = "John", LastName = "Doe" }, StartDate = DateOnly.Parse("2025-03-15") }
        };
        _serviceMock.Setup(s => s.GetFilteredProjectsAsync(parameters))
                    .ReturnsAsync(expectedProjects);

        // Act
        var result = await _controller.GetAll(parameters);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProjects = okResult.Value.Should().BeAssignableTo<IEnumerable<ProjectDetailsDto>>().Subject;
        returnedProjects.Should().BeEquivalentTo(expectedProjects);
    }

    [Fact]
    public async Task GetAll_WithPriorityFilter_ShouldReturnFilteredProjects()
    {
        // Arrange
        var parameters = new ProjectQueryParameters { Priority = 1 };
        var expectedProjects = new List<ProjectDetailsDto>
        {
            new ProjectDetailsDto { Id = 1, Name = "High Priority", Manager = new EmployeeDto { FirstName = "John", LastName = "Doe" }, Priority = 1 },
            new ProjectDetailsDto { Id = 3, Name = "Urgent", Manager = new EmployeeDto { FirstName = "John", LastName = "Doe" }, Priority = 1 }
        };
        _serviceMock.Setup(s => s.GetFilteredProjectsAsync(parameters))
                    .ReturnsAsync(expectedProjects);

        // Act
        var result = await _controller.GetAll(parameters);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProjects = okResult.Value.Should().BeAssignableTo<IEnumerable<ProjectDetailsDto>>().Subject;
        returnedProjects.Should().BeEquivalentTo(expectedProjects);
    }

    [Fact]
    public async Task GetAll_WithStatusFilter_ShouldReturnFilteredProjects()
    {
        // Arrange
        var parameters = new ProjectQueryParameters { Status = "Active" };
        var expectedProjects = new List<ProjectDetailsDto>
        {
            new ProjectDetailsDto { Id = 1, Name = "Active Project", Manager = new EmployeeDto { FirstName = "John", LastName = "Doe" }, Status = "Active" }
        };
        _serviceMock.Setup(s => s.GetFilteredProjectsAsync(parameters))
                    .ReturnsAsync(expectedProjects);

        // Act
        var result = await _controller.GetAll(parameters);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProjects = okResult.Value.Should().BeAssignableTo<IEnumerable<ProjectDetailsDto>>().Subject;
        returnedProjects.Should().BeEquivalentTo(expectedProjects);
    }

    [Fact]
    public async Task GetAll_WithSortByNameAscending_ShouldReturnSortedProjects()
    {
        // Arrange
        var parameters = new ProjectQueryParameters { SortBy = "Name", Descending = false };
        var expectedProjects = new List<ProjectDetailsDto>
        {
            new ProjectDetailsDto { Id = 2, Name = "Alpha", Manager = new EmployeeDto { FirstName = "John", LastName = "Doe" } },
            new ProjectDetailsDto { Id = 1, Name = "Beta", Manager = new EmployeeDto { FirstName = "John", LastName = "Doe" } },
            new ProjectDetailsDto { Id = 3, Name = "Gamma", Manager = new EmployeeDto { FirstName = "John", LastName = "Doe" } }
        };
        _serviceMock.Setup(s => s.GetFilteredProjectsAsync(parameters))
                    .ReturnsAsync(expectedProjects);

        // Act
        var result = await _controller.GetAll(parameters);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProjects = okResult.Value.Should().BeAssignableTo<IEnumerable<ProjectDetailsDto>>().Subject;
        returnedProjects.Should().BeEquivalentTo(expectedProjects);
    }

    [Fact]
    public async Task GetAll_WithSortByPriorityDescending_ShouldReturnSortedProjects()
    {
        // Arrange
        var parameters = new ProjectQueryParameters { SortBy = "Priority", Descending = true };
        var expectedProjects = new List<ProjectDetailsDto>
        {
            new ProjectDetailsDto { Id = 1, Name = "Critical", Manager = new EmployeeDto { FirstName = "John", LastName = "Doe" }, Priority = 3 },
            new ProjectDetailsDto { Id = 2, Name = "High", Manager = new EmployeeDto { FirstName = "John", LastName = "Doe" }, Priority = 2 },
            new ProjectDetailsDto { Id = 3, Name = "Low", Manager = new EmployeeDto { FirstName = "John", LastName = "Doe" }, Priority = 1 }
        };
        _serviceMock.Setup(s => s.GetFilteredProjectsAsync(parameters))
                    .ReturnsAsync(expectedProjects);

        // Act
        var result = await _controller.GetAll(parameters);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProjects = okResult.Value.Should().BeAssignableTo<IEnumerable<ProjectDetailsDto>>().Subject;
        returnedProjects.Should().BeEquivalentTo(expectedProjects);
    }

    [Fact]
    public async Task GetAll_WithSortByStartDate_ShouldReturnSortedProjects()
    {
        // Arrange
        var parameters = new ProjectQueryParameters { SortBy = "StartDate", Descending = false };
        var expectedProjects = new List<ProjectDetailsDto>
        {
            new ProjectDetailsDto { Id = 1, Name = "Early", Manager = new EmployeeDto { FirstName = "John", LastName = "Doe" }, StartDate = DateOnly.Parse("2025-01-01") },
            new ProjectDetailsDto { Id = 2, Name = "Middle", Manager = new EmployeeDto { FirstName = "John", LastName = "Doe" }, StartDate = DateOnly.Parse("2025-06-01") },
            new ProjectDetailsDto { Id = 3, Name = "Late", Manager = new EmployeeDto { FirstName = "John", LastName = "Doe" }, StartDate = DateOnly.Parse("2025-12-01") }
        };
        _serviceMock.Setup(s => s.GetFilteredProjectsAsync(parameters))
                    .ReturnsAsync(expectedProjects);

        // Act
        var result = await _controller.GetAll(parameters);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProjects = okResult.Value.Should().BeAssignableTo<IEnumerable<ProjectDetailsDto>>().Subject;
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
        var expectedProjects = new List<ProjectDetailsDto>
        {
            new ProjectDetailsDto { Id = 1, Name = "Active High Priority", Manager = new EmployeeDto { FirstName = "John", LastName = "Doe" }, Priority = 1, Status = "Active" }
        };
        _serviceMock.Setup(s => s.GetFilteredProjectsAsync(parameters))
                    .ReturnsAsync(expectedProjects);

        // Act
        var result = await _controller.GetAll(parameters);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProjects = okResult.Value.Should().BeAssignableTo<IEnumerable<ProjectDetailsDto>>().Subject;
        returnedProjects.Should().BeEquivalentTo(expectedProjects);
    }

    [Fact]
    public async Task GetAll_WithNoParameters_ShouldReturnAllProjects()
    {
        // Arrange
        var parameters = new ProjectQueryParameters();
        var expectedProjects = new List<ProjectDetailsDto>
        {
            new ProjectDetailsDto { Id = 1, Name = "Project1", Manager = new EmployeeDto { FirstName = "John", LastName = "Doe" } },
            new ProjectDetailsDto { Id = 2, Name = "Project2", Manager = new EmployeeDto { FirstName = "John", LastName = "Doe" } }
        };
        _serviceMock.Setup(s => s.GetFilteredProjectsAsync(parameters))
                    .ReturnsAsync(expectedProjects);

        // Act
        var result = await _controller.GetAll(parameters);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProjects = okResult.Value.Should().BeAssignableTo<IEnumerable<ProjectDetailsDto>>().Subject;
        returnedProjects.Should().BeEquivalentTo(expectedProjects);
    }

    [Fact]
    public async Task GetAll_WithCustomerCompanyFilter_ShouldReturnFilteredProjects()
    {
        // Arrange
        var parameters = new ProjectQueryParameters { CustomerCompanyId = 1 };
        var expectedProjects = new List<ProjectDetailsDto>
        {
            new ProjectDetailsDto { Id = 1, Name = "Company1 Project", Manager = new EmployeeDto { FirstName = "John", LastName = "Doe" }, CustomerCompanyId = 1 }
        };
        _serviceMock.Setup(s => s.GetFilteredProjectsAsync(parameters))
                    .ReturnsAsync(expectedProjects);

        // Act
        var result = await _controller.GetAll(parameters);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProjects = okResult.Value.Should().BeAssignableTo<IEnumerable<ProjectDetailsDto>>().Subject;
        returnedProjects.Should().BeEquivalentTo(expectedProjects);
    }

    [Fact]
    public async Task GetAll_WithManagerFilter_ShouldReturnFilteredProjects()
    {
        // Arrange
        var parameters = new ProjectQueryParameters { ManagerId = 1 };
        var expectedProjects = new List<ProjectDetailsDto>
        {
            new ProjectDetailsDto { Id = 1, Name = "Managed by Ivan", Manager = new EmployeeDto { FirstName = "John", LastName = "Doe" }, ManagerId = 1 }
        };
        _serviceMock.Setup(s => s.GetFilteredProjectsAsync(parameters))
                    .ReturnsAsync(expectedProjects);

        // Act
        var result = await _controller.GetAll(parameters);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProjects = okResult.Value.Should().BeAssignableTo<IEnumerable<ProjectDetailsDto>>().Subject;
        returnedProjects.Should().BeEquivalentTo(expectedProjects);
    }
}
