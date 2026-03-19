using Decryptcode.Assessment.Service.Application.Dashboard.Mappings;
using Decryptcode.Assessment.Service.Domain.Entities.AggregateRoots;
using Decryptcode.Assessment.Service.Domain.Entities.ValueObjects;
using Decryptcode.Assessment.Service.Infrastructure.SqlServer.Contexts;
using Decryptcode.Assessment.Service.Infrastructure.SqlServer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Decryptcode.Assessment.Service.IntegrationTests.Infrastructure.Repositories;

/// <summary>
/// Integration tests for OrganizationRepository
/// Uses in-memory database for data persistence testing
/// </summary>
public class OrganizationRepositoryIntegrationTests : IAsyncLifetime
{
    private readonly ApiContext _context;
    private readonly OrganizationRepository _repository;

    public OrganizationRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApiContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApiContext(options, new HttpContextAccessor());
        _repository = new OrganizationRepository(_context);
    }

    public async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task AddAsync_WithValidOrganization_SavesSuccessfully()
    {
        // Arrange
        var organization = Organization.Create(
            "org-001", "Test Organization", "test-org", "Technology", "enterprise",
            "contact@test.com", "Description",
            new Settings("America/New_York", "USD", true, "en-US"),
            new Metadata("api", null, null));

        // Act
        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync();

        // Assert
        var saved = await _repository.GetByIdAsync("org-001");
        Assert.NotNull(saved);
        Assert.Equal("Test Organization", saved.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsOrganization()
    {
        // Arrange
        var organization = await CreateAndSaveOrganization("org-001", "Test Org");

        // Act
        var retrieved = await _repository.GetByIdAsync("org-001");

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(organization.Name, retrieved.Name);
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
    public async Task GetAllAsync_WithMultipleOrganizations_ReturnsAll()
    {
        // Arrange
        await CreateAndSaveOrganization("org-001", "Org 1");
        await CreateAndSaveOrganization("org-002", "Org 2");
        await CreateAndSaveOrganization("org-003", "Org 3");

        // Act
        var organizations = await _repository.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.Equal(3, organizations.Count());
    }

    [Fact]
    public async Task GetAllFiltered_WithIndustryFilter_ReturnsFiltered()
    {
        // Arrange
        await CreateAndSaveOrganization("org-001", "Tech Org", industry: "Technology");
        await CreateAndSaveOrganization("org-002", "Mfg Org", industry: "Manufacturing");
        await CreateAndSaveOrganization("org-003", "Tech Org 2", industry: "Technology");

        // Act
        var results = await _repository.GetAllFiltered("Technology", null, CancellationToken.None);

        // Assert
        Assert.Equal(2, results.Count());
    }

    [Fact]
    public async Task GetAllFiltered_WithTierFilter_ReturnsFiltered()
    {
        // Arrange
        await CreateAndSaveOrganization("org-001", "Enterprise 1", tier: "enterprise");
        await CreateAndSaveOrganization("org-002", "Professional 1", tier: "professional");
        await CreateAndSaveOrganization("org-003", "Enterprise 2", tier: "enterprise");

        // Act
        var results = await _repository.GetAllFiltered(null, "enterprise", CancellationToken.None);

        // Assert
        Assert.Equal(2, results.Count());
    }

    [Fact]
    public async Task GetAllFiltered_WithBothFilters_ReturnsFiltered()
    {
        // Arrange
        await CreateAndSaveOrganization("org-001", "Tech Enterprise", "Technology", "enterprise");
        await CreateAndSaveOrganization("org-002", "Mfg Professional", "Manufacturing", "professional");
        await CreateAndSaveOrganization("org-003", "Tech Professional", "Technology", "professional");

        // Act
        var results = await _repository.GetAllFiltered("Technology", "enterprise", CancellationToken.None);

        // Assert
        Assert.Single(results);
        Assert.Equal("Tech Enterprise", results.First().Name);
    }

    [Fact]
    public async Task GetAllFiltered_WithNoMatches_ReturnsEmpty()
    {
        // Arrange
        await CreateAndSaveOrganization("org-001", "Test Org");

        // Act
        var results = await _repository.GetAllFiltered("NonExistent", null, CancellationToken.None);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public async Task GetDashboardAsync_ReturnsAggregatedData()
    {
        // Arrange
        await CreateAndSaveOrganization("org-001", "Org 1");
        await CreateAndSaveOrganization("org-002", "Org 2");

        // Act
        var dashboard = await _repository.GetDashboardAsync(
            DashboardMappings.Projection,
            CancellationToken.None);

        // Assert
        Assert.NotNull(dashboard);
        Assert.Equal(2, dashboard.TotalOrganizations);
    }

    // Helper methods
    private async Task<Organization> CreateAndSaveOrganization(
        string id, string name, string industry = "Technology", string tier = "professional")
    {
        var org = Organization.Create(
            id, name, id.ToLower(), industry, tier,
            $"contact@{id}.com", "Test organization",
            new Settings("America/New_York", "USD", true, "en-US"),
            new Metadata("api", null, null));

        _context.Organizations.Add(org);
        await _context.SaveChangesAsync();
        return org;
    }
}
