using Decryptcode.Assessment.Service.Application.Organizations.Queries.GetOrganizationById;
using Decryptcode.Assessment.Service.Domain.Entities.AggregateRoots;
using Decryptcode.Assessment.Service.Domain.Entities.ValueObjects;
using Decryptcode.Assessment.Service.Domain.Repositories;
using Moq;
using Xunit;

namespace Decryptcode.Assessment.Service.Domain.UnitTests.Application.Organizations;

/// <summary>
/// Unit tests for GetOrganizationByIdQueryHandler
/// Tests retrieving individual organization by ID
/// </summary>
public class GetOrganizationByIdQueryHandlerTests
{
    private readonly Mock<IOrganizationRepository> _repositoryMock;
    private readonly GetOrganizationByIdQueryHandler _handler;

    public GetOrganizationByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IOrganizationRepository>();
        _handler = new GetOrganizationByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidId_ReturnsOrganization()
    {
        // Arrange
        var organizationId = "org-001";
        var query = new GetOrganizationByIdQuery { Id = organizationId };
        var organization = CreateTestOrganization(organizationId, "Acme Corp", "Technology");

        _repositoryMock.Setup(r => r.GetByIdAsync(organizationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(organization);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Content);
        Assert.Equal("Acme Corp", result.Content.Name);
        _repositoryMock.Verify(r => r.GetByIdAsync(organizationId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var query = new GetOrganizationByIdQuery { Id = "invalid-id" };
        Organization? nullOrganization = null;

        _repositoryMock.Setup(r => r.GetByIdAsync("invalid-id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(nullOrganization);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.Null(result.Content);
    }

    [Fact]
    public async Task Handle_WithNullId_ReturnsNotFound()
    {
        // Arrange
        var query = new GetOrganizationByIdQuery { Id = null };
        Organization? nullOrganization = null;

        _repositoryMock.Setup(r => r.GetByIdAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(nullOrganization);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task Handle_WithRepositoryException_HandlesGracefully()
    {
        // Arrange
        var query = new GetOrganizationByIdQuery { Id = "org-001" };
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ReturnsCorrectlyMappedDto()
    {
        // Arrange
        var organizationId = "org-001";
        var query = new GetOrganizationByIdQuery { Id = organizationId };
        var organization = CreateTestOrganization(organizationId, "Test Org", "Healthcare");

        _repositoryMock.Setup(r => r.GetByIdAsync(organizationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(organization);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result.Content);
        Assert.Equal("Test Org", result.Content.Name);
        Assert.Equal("org-001", result.Content.Slug);
        Assert.Equal("Healthcare", result.Content.Industry);
    }

    // Helper methods
    private Organization CreateTestOrganization(string id, string name, string industry) =>
        Organization.Create(
            id, name, id.ToLower(), industry, "enterprise",
            $"contact@{id}.com", "Test organization",
            new Settings("America/New_York", "USD", true, "en-US"),
            new Metadata("api", null, null));
}
