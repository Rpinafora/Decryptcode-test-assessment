using Decryptcode.Assessment.Service.Application.Dashboard.Dtos;
using Decryptcode.Assessment.Service.Application.Dashboard.Mappings;
using Decryptcode.Assessment.Service.Application.Organizations.Mappings;
using Decryptcode.Assessment.Service.Application.Projects.Mappings;
using Decryptcode.Assessment.Service.Application.Users.Mappings;
using Decryptcode.Assessment.Service.Domain.Entities.AggregateRoots;
using Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;
using Decryptcode.Assessment.Service.Domain.Entities.ValueObjects;
using Xunit;

namespace Decryptcode.Assessment.Service.Domain.UnitTests.Application.Mappings;

/// <summary>
/// Unit tests for OrganizationMappings, ProjectMappings, UserMapping, and DashboardMappings
/// </summary>
public class MappingTests
{
    [Fact]
    public void OrganizationMapping_WithValidOrganization_ReturnsDto()
    {
        // Arrange
        var organization = Organization.Create(
            "org-001", "Test Organization", "test-org", "Technology", "enterprise",
            "contact@test.com", "A test organization",
            new Settings("America/New_York", "USD", true, "en-US"),
            new Metadata("migration", 1001, DateTime.UtcNow));

        // Act
        var projection = OrganizationMappings.Projection.Compile();
        var dto = projection(organization);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(organization.Id, dto.Id);
        Assert.Equal(organization.Name, dto.Name);
        Assert.Equal(organization.Slug, dto.Slug);
        Assert.Equal(organization.Industry, dto.Industry);
    }

    [Fact]
    public void OrganizationMapping_MapsSettingsCorrectly()
    {
        // Arrange
        var organization = Organization.Create(
            "org-001", "Test Org", "test-org", "Technology", "enterprise",
            "contact@test.com", "Description",
            new Settings("America/New_York", "USD", true, "en-US"),
            new Metadata("api", null, null));

        // Act
        var projection = OrganizationMappings.Projection.Compile();
        var dto = projection(organization);

        // Assert
        Assert.NotNull(dto.Settings);
        Assert.Equal("America/New_York", dto.Settings.Timezone);
        Assert.Equal("USD", dto.Settings.Currency);
    }

    [Fact]
    public void OrganizationMapping_MapsMetadataCorrectly()
    {
        // Arrange
        var organization = Organization.Create(
            "org-001", "Test Org", "test-org", "Technology", "enterprise",
            "contact@test.com", "Description",
            new Settings("America/New_York", "USD", true, "en-US"),
            new Metadata("api", 1001, DateTime.UtcNow));

        // Act
        var projection = OrganizationMappings.Projection.Compile();
        var dto = projection(organization);

        // Assert
        Assert.NotNull(dto.Metadata);
        Assert.Equal("api", dto.Metadata.Source);
    }

    [Fact]
    public void ProjectMapping_WithValidProject_ReturnsDto()
    {
        // Arrange
        var project = Domain.Entities.ReferenceEntities.Project.Create(
            "proj-001", "org-001", "Test Project", Domain.Enums.ProjectStatus.Active, 100,
            DateTime.Parse("2024-01-01"), DateTime.Parse("2024-12-31"), "Description");

        // Act
        var projection = ProjectMappings.Projection.Compile();
        var dto = projection(project);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(project.Id, dto.Id);
        Assert.Equal(project.Name, dto.Name);
        Assert.Equal("active", dto.Status);
    }

    [Fact]
    public void ProjectMapping_ConvertsDatesCorrectly()
    {
        // Arrange
        var project = Domain.Entities.ReferenceEntities.Project.Create(
            "proj-001", "org-001", "Test Project", Domain.Enums.ProjectStatus.Active, 100,
            DateTime.Parse("2024-01-01"), DateTime.Parse("2024-12-31"), "Description");

        // Act
        var projection = ProjectMappings.Projection.Compile();
        var dto = projection(project);

        // Assert
        Assert.NotNull(dto.StartDate);
        Assert.NotNull(dto.EndDate);
    }

    [Fact]
    public void ProjectMapping_WithNullDates_ReturnsNullDates()
    {
        // Arrange
        var project = Domain.Entities.ReferenceEntities.Project.Create(
            "proj-001", "org-001", "Test Project", Domain.Enums.ProjectStatus.Planned, 0,
            null, null, "Description");

        // Act
        var projection = ProjectMappings.Projection.Compile();
        var dto = projection(project);

        // Assert
        Assert.Null(dto.StartDate);
        Assert.Null(dto.EndDate);
    }

    [Fact]
    public void UserMapping_WithValidUser_ReturnsDto()
    {
        // Arrange
        var user = User.Create("user-001", "org-001", "test@email.com", "Test User", "admin", true, "Test bio");

        // Act
        var projection = UserMapping.UserProjection.Compile();
        var dto = projection(user);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(user.Id, dto.Id);
        Assert.Equal(user.Name, dto.Name);
        Assert.Equal(user.Email, dto.Email);
        Assert.Equal("admin", dto.Role);
    }

    [Fact]
    public void UserMapping_WithInactiveUser_ReturnsInactiveDto()
    {
        // Arrange
        var user = User.Create("user-001", "org-001", "test@email.com", "Test User", "member", false, "Bio");

        // Act
        var projection = UserMapping.UserProjection.Compile();
        var dto = projection(user);

        // Assert
        Assert.False(dto.Active);
    }

    [Fact]
    public void DashboardMapping_WithValidOrganizations_ReturnsAggregates()
    {
        // Arrange
        var orgs = new[]
        {
            Organization.Create("org-001", "Organization 1", "org-001", "Technology", "enterprise",
                "contact@org-001.com", "Description",
                new Settings("America/New_York", "USD", true, "en-US"),
                new Metadata("api", null, null)),
            Organization.Create("org-002", "Organization 2", "org-002", "Technology", "enterprise",
                "contact@org-002.com", "Description",
                new Settings("America/New_York", "USD", true, "en-US"),
                new Metadata("api", null, null))
        };

        // Act
        var projection = DashboardMappings.Projection.Compile();
        var dtoQueryable = projection(orgs.AsQueryable());
        var dto = dtoQueryable.FirstOrDefault();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(2, dto.TotalOrganizations);
    }

    [Fact]
    public void DashboardMapping_ReturnsQueryable()
    {
        // Arrange
        var orgs = new[]
        {
            Organization.Create("org-001", "Organization 1", "org-001", "Technology", "enterprise",
                "contact@org-001.com", "Description",
                new Settings("America/New_York", "USD", true, "en-US"),
                new Metadata("api", null, null))
        };

        // Act
        var projection = DashboardMappings.Projection.Compile();
        var result = projection(orgs.AsQueryable());

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IQueryable<DashboardDto>>(result);
    }
}
