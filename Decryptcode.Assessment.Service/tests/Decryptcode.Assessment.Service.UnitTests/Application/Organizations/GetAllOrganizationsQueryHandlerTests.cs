using Decryptcode.Assessment.Service.Application.Organizations.Queries.GetAllOrganizations;
using Decryptcode.Assessment.Service.Domain.Entities.AggregateRoots;
using Decryptcode.Assessment.Service.Domain.Entities.ValueObjects;
using Decryptcode.Assessment.Service.Domain.Repositories;
using Moq;
using Xunit;

namespace Decryptcode.Assessment.Service.Domain.UnitTests.Application.Organizations;

/// <summary>
/// Unit tests for GetAllOrganizationsQueryHandler
/// Tests retrieving organizations with filtering capabilities
/// </summary>
public class GetAllOrganizationsQueryHandlerTests
{
    private readonly Mock<IOrganizationRepository> _repositoryMock;
    private readonly GetAllOrganizationsQueryHandler _handler;

    public GetAllOrganizationsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IOrganizationRepository>();
        _handler = new GetAllOrganizationsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ReturnsOrganizations()
    {
        // Arrange
        var query = new GetAllOrganizationsQuery { Industry = null, Tier = null };
        var organizations = new[]
        {
            CreateTestOrganization("org-001", "Acme Corp", "Technology"),
            CreateTestOrganization("org-002", "Beta Industries", "Manufacturing")
        };

        _repositoryMock.Setup(r => r.GetAllFiltered(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(organizations);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Content);
        Assert.Equal(2, result.Content.Count());
        _repositoryMock.Verify(r => r.GetAllFiltered(null, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithIndustryFilter_ReturnsFilteredOrganizations()
    {
        // Arrange
        var query = new GetAllOrganizationsQuery { Industry = "Technology", Tier = null };
        var organizations = new[]
        {
            CreateTestOrganization("org-001", "Acme Corp", "Technology")
        };

        _repositoryMock.Setup(r => r.GetAllFiltered("Technology", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(organizations);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Single(result.Content!);
        _repositoryMock.Verify(r => r.GetAllFiltered("Technology", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithTierFilter_ReturnsFilteredOrganizations()
    {
        // Arrange
        var query = new GetAllOrganizationsQuery { Industry = null, Tier = "enterprise" };
        var organizations = new[]
        {
            CreateTestOrganization("org-001", "Acme Corp", "Technology"),
            CreateTestOrganization("org-004", "Delta Financial", "Financial Services")
        };

        _repositoryMock.Setup(r => r.GetAllFiltered(null, "enterprise", It.IsAny<CancellationToken>()))
            .ReturnsAsync(organizations);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(2, result.Content!.Count());
    }

    [Fact]
    public async Task Handle_WithBothFilters_ReturnsFilteredOrganizations()
    {
        // Arrange
        var query = new GetAllOrganizationsQuery { Industry = "Technology", Tier = "enterprise" };
        var organizations = new[]
        {
            CreateTestOrganization("org-001", "Acme Corp", "Technology")
        };

        _repositoryMock.Setup(r => r.GetAllFiltered("Technology", "enterprise", It.IsAny<CancellationToken>()))
            .ReturnsAsync(organizations);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Single(result.Content!);
        _repositoryMock.Verify(r => r.GetAllFiltered("Technology", "enterprise", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNoMatches_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllOrganizationsQuery { Industry = "NonExistent", Tier = null };
        var organizations = Array.Empty<Organization>();

        _repositoryMock.Setup(r => r.GetAllFiltered("NonExistent", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(organizations);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Empty(result.Content!);
    }

    [Fact]
    public async Task Handle_WithRepositoryException_HandlesGracefully()
    {
        // Arrange
        var query = new GetAllOrganizationsQuery();
        _repositoryMock.Setup(r => r.GetAllFiltered(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }

    // Helper methods
    private Organization CreateTestOrganization(string id, string name, string industry) =>
        Organization.Create(
            id, name, id.ToLower(), industry, "enterprise",
            $"contact@{id}.com", "Test organization",
            new Settings("America/New_York", "USD", true, "en-US"),
            new Metadata("api", null, null));
}