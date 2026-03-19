using Decryptcode.Assessment.Service.Application.Projects.Dtos;
using Decryptcode.Assessment.Service.Application.Projects.GetAllProjects;
using Decryptcode.Assessment.Service.Application.Projects.GetProjectById;
using Decryptcode.Assessment.Service.Application.Organizations.Dtos;
using Decryptcode.Assessment.Service.Application;
using Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;
using Decryptcode.Assessment.Service.Domain.Enums;
using Decryptcode.Assessment.Service.Domain.Repositories;
using Moq;
using System.Linq.Expressions;
using Xunit;
using Decryptcode.Assessment.Service.Application;

namespace Decryptcode.Assessment.Service.Domain.UnitTests.Application.Projects;

/// <summary>
/// Unit tests for GetAllProjectsQueryHandler and GetProjectByIdQueryHandler
/// </summary>
public class ProjectQueryHandlerTests
{
    [Fact]
    public async Task GetAllProjects_WithValidQuery_ReturnsAllProjects()
    {
        // Arrange
        var repositoryMock = new Mock<IProjectRepository>();
        var handler = new GetAllProjectsQueryHandler(repositoryMock.Object);
        var query = new GetAllProjectsQuery();
        var projects = new[]
        {
            CreateTestProject("proj-001", "Platform Modernization", ProjectStatus.Active),
            CreateTestProject("proj-002", "API Gateway", ProjectStatus.Active),
            CreateTestProject("proj-003", "Legacy Audit", ProjectStatus.Completed)
        };

        var projectDtos = new List<ProjectDto>
        {
            new ProjectDto 
            { 
                Id = projects[0].Id, 
                OrgId = "org-001", 
                Name = projects[0].Name, 
                Status = "Active", 
                BudgetHours = projects[0].BudgetHours, 
                StartDate = projects[0].StartDate.HasValue ? DateOnly.FromDateTime(projects[0].StartDate.Value) : null, 
                EndDate = projects[0].EndDate.HasValue ? DateOnly.FromDateTime(projects[0].EndDate.Value) : null, 
                Description = projects[0].Description 
            },
            new ProjectDto 
            { 
                Id = projects[1].Id, 
                OrgId = "org-001", 
                Name = projects[1].Name, 
                Status = "Active", 
                BudgetHours = projects[1].BudgetHours, 
                StartDate = projects[1].StartDate.HasValue ? DateOnly.FromDateTime(projects[1].StartDate.Value) : null, 
                EndDate = projects[1].EndDate.HasValue ? DateOnly.FromDateTime(projects[1].EndDate.Value) : null, 
                Description = projects[1].Description 
            },
            new ProjectDto 
            { 
                Id = projects[2].Id, 
                OrgId = "org-001", 
                Name = projects[2].Name, 
                Status = "Completed", 
                BudgetHours = projects[2].BudgetHours, 
                StartDate = projects[2].StartDate.HasValue ? DateOnly.FromDateTime(projects[2].StartDate.Value) : null, 
                EndDate = projects[2].EndDate.HasValue ? DateOnly.FromDateTime(projects[2].EndDate.Value) : null, 
                Description = projects[2].Description 
            }
        };

        repositoryMock.Setup(r => r.GetAllFiltered(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<Expression<Func<Project, ProjectDto>>>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectDtos);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Content);
        Assert.Equal(3, result.Content.Count());
        repositoryMock.Verify(r => r.GetAllFiltered(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Expression<Func<Project, ProjectDto>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllProjects_WithNoProjects_ReturnsEmptyList()
    {
        // Arrange
        var repositoryMock = new Mock<IProjectRepository>();
        var handler = new GetAllProjectsQueryHandler(repositoryMock.Object);
        var query = new GetAllProjectsQuery();
        var emptyDtos = Enumerable.Empty<ProjectDto>();

        repositoryMock.Setup(r => r.GetAllFiltered(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<Expression<Func<Project, ProjectDto>>>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyDtos);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Empty(result.Content!);
    }

    [Fact]
    public async Task GetProjectById_WithValidId_ReturnsProjectWithOrganization()
    {
        // Arrange
        var repositoryMock = new Mock<IProjectRepository>();
        var handler = new GetProjectByIdQueryHandler(repositoryMock.Object);
        var projectId = "proj-001";
        var query = new GetProjectByIdQuery { Id = projectId };
        var project = CreateTestProject(projectId, "Platform Modernization", ProjectStatus.Active);

        var orgSettingsDto = new SettingsDto("America/New_York", "USD", true, "en-US");
        var orgMetadataDto = new MetadataDto("api", null, null);

        var orgDto = new OrganizationDto 
        { 
            Id = "org-001", 
            Name = "Test Org", 
            Slug = "test-org", 
            Industry = "Technology", 
            Tier = "enterprise",
            ContactEmail = "contact@test.com",
            CreatedAt = DateTime.UtcNow,
            Settings = orgSettingsDto,
            Metadata = orgMetadataDto
        };

        var projectSummaryDto = new ProjectSummaryDto 
        { 
            Organization = orgDto,
            Name = project.Name, 
            Status = "Active", 
            BudgetHours = project.BudgetHours, 
            StartDate = project.StartDate.HasValue ? DateOnly.FromDateTime(project.StartDate.Value) : null, 
            EndDate = project.EndDate.HasValue ? DateOnly.FromDateTime(project.EndDate.Value) : null,
            TotalHoursLogged = 0
        };

        repositoryMock.Setup(r => r.GetByIdAsync(projectId, It.IsAny<Expression<Func<Project, ProjectSummaryDto>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectSummaryDto);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Content);
        Assert.Equal("Platform Modernization", result.Content.Name);
    }

    [Fact]
    public async Task GetProjectById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var repositoryMock = new Mock<IProjectRepository>();
        var handler = new GetProjectByIdQueryHandler(repositoryMock.Object);
        var query = new GetProjectByIdQuery { Id = "invalid-id" };
        ProjectSummaryDto? nullDto = null;

        repositoryMock.Setup(r => r.GetByIdAsync("invalid-id", It.IsAny<Expression<Func<Project, ProjectSummaryDto>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(nullDto);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.Null(result.Content);
    }

    // Helper methods
    private Project CreateTestProject(string id, string name, ProjectStatus status) =>
        Project.Create(
            id, "org-001", name, status, 100,
            DateTime.Parse("2024-01-01"), DateTime.Parse("2024-12-31"),
            $"Description for {name}");
}
