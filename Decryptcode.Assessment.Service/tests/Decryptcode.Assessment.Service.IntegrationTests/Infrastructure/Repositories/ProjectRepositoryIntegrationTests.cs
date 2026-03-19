using Decryptcode.Assessment.Service.Domain.Entities.AggregateRoots;
using Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;
using Decryptcode.Assessment.Service.Domain.Entities.ValueObjects;
using Decryptcode.Assessment.Service.Domain.Enums;
using Decryptcode.Assessment.Service.Infrastructure.SqlServer.Contexts;
using Decryptcode.Assessment.Service.Infrastructure.SqlServer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Decryptcode.Assessment.Service.IntegrationTests.Infrastructure.Repositories;

/// <summary>
/// Integration tests for ProjectRepository
/// Tests project persistence and retrieval with relationships
/// </summary>
public class ProjectRepositoryIntegrationTests : IAsyncLifetime
{
    private readonly ApiContext _context;
    private readonly ProjectRepository _repository;

    public ProjectRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApiContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApiContext(options, new HttpContextAccessor());
        _repository = new ProjectRepository(_context);
    }

    public async Task InitializeAsync()
    {
        // Create test organization
        var testOrganization = Organization.Create(
            "org-001", "Test Organization", "test-org", "Technology", "enterprise",
            "contact@test.com", "Description",
            new Settings("America/New_York", "USD", true, "en-US"),
            new Metadata("api", null, null));

        _context.Organizations.Add(testOrganization);
        await _context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task AddAsync_WithValidProject_SavesSuccessfully()
    {
        // Arrange
        var project = Project.Create(
            "proj-001", "org-001", "Test Project", ProjectStatus.Active, 100,
            DateTime.Parse("2024-01-01"), DateTime.Parse("2024-12-31"), "Description");

        // Act
        _context.Set<Project>().Add(project);
        await _context.SaveChangesAsync();

        // Assert
        var saved = await _repository.GetByIdAsync("proj-001");
        Assert.NotNull(saved);
        Assert.Equal("Test Project", saved.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsProject()
    {
        // Arrange
        var project = await CreateAndSaveProject("proj-001", "Test Project", ProjectStatus.Active);

        // Act
        var retrieved = await _repository.GetByIdAsync("proj-001");

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(project.Name, retrieved.Name);
        Assert.Equal(ProjectStatus.Active, retrieved.Status);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync("non-existent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleProjects_ReturnsAll()
    {
        // Arrange
        await CreateAndSaveProject("proj-001", "Project 1", ProjectStatus.Active);
        await CreateAndSaveProject("proj-002", "Project 2", ProjectStatus.Planned);
        await CreateAndSaveProject("proj-003", "Project 3", ProjectStatus.Completed);

        // Act
        var projects = await _repository.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.Equal(3, projects.Count());
    }

    [Fact]
    public async Task GetAllAsync_WithNoProjects_ReturnsEmpty()
    {
        // Act
        var projects = await _repository.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.Empty(projects);
    }

    [Fact]
    public async Task Project_WithNullDates_PersistsCorrectly()
    {
        // Arrange
        var project = Project.Create(
            "proj-001", "org-001", "Planned Project", ProjectStatus.Planned, 0,
            null, null, "Future project");

        // Act
        _context.Set<Project>().Add(project);
        await _context.SaveChangesAsync();
        var retrieved = await _repository.GetByIdAsync("proj-001");

        // Assert
        Assert.NotNull(retrieved);
        Assert.Null(retrieved.StartDate);
        Assert.Null(retrieved.EndDate);
    }

    [Fact]
    public async Task Project_StatusTransitions_PersistCorrectly()
    {
        // Arrange
        var project = await CreateAndSaveProject("proj-001", "Test", ProjectStatus.Planned);

        // Act
        project.Activate();
        _context.Set<Project>().Update(project);
        await _context.SaveChangesAsync();

        var retrieved = await _repository.GetByIdAsync("proj-001");

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(ProjectStatus.Active, retrieved.Status);
    }

    // Helper methods
    private async Task<Project> CreateAndSaveProject(
        string id, string name, ProjectStatus status)
    {
        var project = Project.Create(
            id, "org-001", name, status, 100,
            DateTime.Parse("2024-01-01"), DateTime.Parse("2024-12-31"), "Description");

        _context.Set<Project>().Add(project);
        await _context.SaveChangesAsync();
        return project;
    }
}
