using Microsoft.AspNetCore.Mvc;
using Moq;
using sibersProject.Controllers;
using sibersProject.Data.DTO;
using sibersProject.Services;
using FluentAssertions;
using Xunit;

namespace sibersProject.Tests;

public class EmployeesControllerTests
{
    private readonly Mock<IEmployeeService> _serviceMock;
    private readonly EmployeesController _controller;

    public EmployeesControllerTests()
    {
        _serviceMock = new Mock<IEmployeeService>();
        _controller = new EmployeesController(_serviceMock.Object);
    }

    // GET /api/employees (GetAll)
    [Fact]
    public async Task GetAll_ShouldReturnOk_WithListOfEmployees()
    {
        // Arrange
        var expectedEmployees = new List<EmployeeDto>
        {
            new EmployeeDto { Id = 1, FirstName = "Ivan", LastName = "Ivanov", Email = "ivan@example.com", IsActive = 1 },
            new EmployeeDto { Id = 2, FirstName = "Anna", LastName = "Petrova", Email = "anna@example.com", IsActive = 1 }
        };
        _serviceMock.Setup(s => s.GetAllEmployeesAsync())
                    .ReturnsAsync(expectedEmployees);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedEmployees = okResult.Value.Should().BeAssignableTo<IEnumerable<EmployeeDto>>().Subject;
        returnedEmployees.Should().BeEquivalentTo(expectedEmployees);
    }

    // GET /api/employees/{id} (GetById)
    [Fact]
    public async Task GetById_WhenEmployeeExists_ShouldReturnOk()
    {
        // Arrange
        var employee = new EmployeeDto { Id = 1, FirstName = "Ivan", LastName = "Ivanov", Email = "ivan@example.com", IsActive = 1 };
        _serviceMock.Setup(s => s.GetEmployeeByIdAsync(1))
                    .ReturnsAsync(employee);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(employee);
    }

    [Fact]
    public async Task GetById_WhenEmployeeDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetEmployeeByIdAsync(99))
                    .ReturnsAsync((EmployeeDto?)null);

        // Act
        var result = await _controller.GetById(99);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    // POST /api/employees (Create)
    [Fact]
    public async Task Create_WithValidDto_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateEmployeeDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com"
        };
        var createdEmployee = new EmployeeDto { Id = 5, FirstName = "Test", LastName = "User", Email = "test@example.com", IsActive = 1 };

        _serviceMock.Setup(s => s.CreateEmployeeAsync(createDto))
                    .ReturnsAsync(createdEmployee);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(EmployeesController.GetById));
        createdResult.RouteValues["id"].Should().Be(5);
        createdResult.Value.Should().BeEquivalentTo(createdEmployee);
    }

    [Fact]
    public async Task Create_WhenServiceThrowsArgumentException_ShouldReturnBadRequest()
    {
        // Arrange
        var createDto = new CreateEmployeeDto { FirstName = "Invalid", LastName = "User", Email = "invalid@example.com" };
        var errorMessage = "Employee with this email already exists";
        _serviceMock.Setup(s => s.CreateEmployeeAsync(createDto))
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

    // PUT /api/employees/{id} (Update)
    [Fact]
    public async Task Update_WhenSuccessful_ShouldReturnNoContent()
    {
        // Arrange
        var updateDto = new UpdateEmployeeDto { FirstName = "Updated", LastName = "User", Email = "updated@example.com", IsActive = 1 };
        _serviceMock.Setup(s => s.UpdateEmployeeAsync(1, updateDto))
                    .ReturnsAsync(true);

        // Act
        var result = await _controller.Update(1, updateDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_WhenEmployeeNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var updateDto = new UpdateEmployeeDto { FirstName = "Updated", LastName = "User", Email = "updated@example.com", IsActive = 1 };
        _serviceMock.Setup(s => s.UpdateEmployeeAsync(99, updateDto))
                    .ReturnsAsync(false);

        // Act
        var result = await _controller.Update(99, updateDto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    // DELETE /api/employees/{id}
    [Fact]
    public async Task Delete_WhenExists_ShouldReturnNoContent()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteEmployeeAsync(1))
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
        _serviceMock.Setup(s => s.DeleteEmployeeAsync(99))
                    .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(99);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    // POST /api/employees/{employeeId}/projects/{projectId}
    [Fact]
    public async Task AddToProject_WhenSuccessful_ShouldReturnOk()
    {
        // Arrange
        _serviceMock.Setup(s => s.AddEmployeeToProjectAsync(2, 1, null))
                    .ReturnsAsync(true);

        // Act
        var result = await _controller.AddToProject(1, 2);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { message = "Employee added to project successfully" });
    }

    [Fact]
    public async Task AddToProject_WhenServiceThrowsArgumentException_ShouldReturnBadRequest()
    {
        // Arrange
        var errorMessage = "Project or Employee not found";
        _serviceMock.Setup(s => s.AddEmployeeToProjectAsync(99, 1, null))
                    .ThrowsAsync(new ArgumentException(errorMessage));

        // Act
        var result = await _controller.AddToProject(1, 99);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var value = badRequestResult.Value;
        value.Should().NotBeNull();
        var property = value!.GetType().GetProperty("error");
        property.Should().NotBeNull();
        property!.GetValue(value).Should().Be(errorMessage);
    }

    // DELETE /api/employees/{employeeId}/projects/{projectId}
    [Fact]
    public async Task RemoveFromProject_WhenSuccessful_ShouldReturnOk()
    {
        // Arrange
        _serviceMock.Setup(s => s.RemoveEmployeeFromProjectAsync(1, 1))
                    .ReturnsAsync(true);

        // Act
        var result = await _controller.RemoveFromProject(1, 1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { message = "Employee removed from project successfully" });
    }

    [Fact]
    public async Task RemoveFromProject_WhenServiceThrowsArgumentException_ShouldReturnBadRequest()
    {
        // Arrange
        var errorMessage = "Employee is not assigned to this project";
        _serviceMock.Setup(s => s.RemoveEmployeeFromProjectAsync(99, 1))
                    .ThrowsAsync(new ArgumentException(errorMessage));

        // Act
        var result = await _controller.RemoveFromProject(1, 99);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var value = badRequestResult.Value;
        value.Should().NotBeNull();
        var property = value!.GetType().GetProperty("error");
        property.Should().NotBeNull();
        property!.GetValue(value).Should().Be(errorMessage);
    }

    // GET /api/employees/by-project/{projectId}
    [Fact]
    public async Task GetByProjectId_ShouldReturnOk_WithListOfEmployees()
    {
        // Arrange
        var expectedEmployees = new List<EmployeeDto>
        {
            new EmployeeDto { Id = 1, FirstName = "Ivan", LastName = "Ivanov", Email = "ivan@example.com", IsActive = 1 }
        };
        _serviceMock.Setup(s => s.GetEmployeesByProjectIdAsync(1))
                    .ReturnsAsync(expectedEmployees);

        // Act
        var result = await _controller.GetByProjectId(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedEmployees = okResult.Value.Should().BeAssignableTo<IEnumerable<EmployeeDto>>().Subject;
        returnedEmployees.Should().BeEquivalentTo(expectedEmployees);
    }

    [Fact]
    public async Task GetByProjectId_WhenServiceThrowsArgumentException_ShouldReturnBadRequest()
    {
        // Arrange
        var errorMessage = "Project not found";
        _serviceMock.Setup(s => s.GetEmployeesByProjectIdAsync(99))
                    .ThrowsAsync(new ArgumentException(errorMessage));

        // Act
        var result = await _controller.GetByProjectId(99);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var value = badRequestResult.Value;
        value.Should().NotBeNull();
        var property = value!.GetType().GetProperty("error");
        property.Should().NotBeNull();
        property!.GetValue(value).Should().Be(errorMessage);
    }

    // GET /api/employees/{employeeId}/projects
    [Fact]
    public async Task GetProjectsByEmployee_ShouldReturnOk_WithListOfProjects()
    {
        // Arrange
        var expectedProjects = new List<ProjectDto>
        {
            new ProjectDto { Id = 1, Name = "Project1", StartDate = DateOnly.Parse("2025-01-01") }
        };
        _serviceMock.Setup(s => s.GetProjectsByEmployeeIdAsync(1))
                    .ReturnsAsync(expectedProjects);

        // Act
        var result = await _controller.GetProjectsByEmployee(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProjects = okResult.Value.Should().BeAssignableTo<IEnumerable<ProjectDto>>().Subject;
        returnedProjects.Should().BeEquivalentTo(expectedProjects);
    }

    [Fact]
    public async Task GetProjectsByEmployee_WhenServiceThrowsArgumentException_ShouldReturnBadRequest()
    {
        // Arrange
        var errorMessage = "Employee not found";
        _serviceMock.Setup(s => s.GetProjectsByEmployeeIdAsync(99))
                    .ThrowsAsync(new ArgumentException(errorMessage));

        // Act
        var result = await _controller.GetProjectsByEmployee(99);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var value = badRequestResult.Value;
        value.Should().NotBeNull();
        var property = value!.GetType().GetProperty("error");
        property.Should().NotBeNull();
        property!.GetValue(value).Should().Be(errorMessage);
    }
}
