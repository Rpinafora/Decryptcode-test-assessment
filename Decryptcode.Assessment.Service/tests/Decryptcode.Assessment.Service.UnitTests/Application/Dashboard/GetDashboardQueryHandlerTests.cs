using Decryptcode.Assessment.Service.Application.Dashboard;
using Decryptcode.Assessment.Service.Application.Dashboard.Dtos;
using Decryptcode.Assessment.Service.Application.Dashboard.Mappings;
using Decryptcode.Assessment.Service.Domain.Entities.AggregateRoots;
using Decryptcode.Assessment.Service.Domain.Repositories;
using Moq;
using Xunit;

namespace Decryptcode.Assessment.Service.Domain.UnitTests.Application.Dashboard;

/// <summary>
/// Unit tests for GetDashboardQueryHandler
/// Tests dashboard aggregation query execution
/// </summary>
public class GetDashboardQueryHandlerTests
{
    private readonly Mock<IOrganizationRepository> _repositoryMock;
    private readonly GetDashboardQueryHandler _handler;

    public GetDashboardQueryHandlerTests()
    {
        _repositoryMock = new Mock<IOrganizationRepository>();
        _handler = new GetDashboardQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ReturnsDashboard()
    {
        // Arrange
        var query = new GetDashboardQuery();
        var expectedDashboard = new DashboardDto
        {
            TotalOrganizations = 4,
            TotalUsers = 8,
            TotalProjects = 6,
            ActiveProjects = 4,
            TotalTimeEntries = 12,
            TotalInvoiced = 137000m
        };

        _repositoryMock.Setup(r => r.GetDashboardAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<
                    IQueryable<Organization>,
                    IQueryable<DashboardDto>>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDashboard);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Content);
        Assert.Equal(4, result.Content.TotalOrganizations);
        Assert.Equal(8, result.Content.TotalUsers);
        Assert.Equal(6, result.Content.TotalProjects);
        _repositoryMock.Verify(r => r.GetDashboardAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<
                IQueryable<Organization>,
                IQueryable<DashboardDto>>>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithRepositoryException_HandlesGracefully()
    {
        // Arrange
        var query = new GetDashboardQuery();
        _repositoryMock.Setup(r => r.GetDashboardAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<
                    IQueryable<Organization>,
                    IQueryable<DashboardDto>>>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithZeroData_ReturnsZeroDashboard()
    {
        // Arrange
        var query = new GetDashboardQuery();
        var emptyDashboard = new DashboardDto
        {
            TotalOrganizations = 0,
            TotalUsers = 0,
            TotalProjects = 0,
            ActiveProjects = 0,
            TotalTimeEntries = 0,
            TotalInvoiced = 0
        };

        _repositoryMock.Setup(r => r.GetDashboardAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<
                    IQueryable<Domain.Entities.AggregateRoots.Organization>,
                    IQueryable<DashboardDto>>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyDashboard);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Content);
        Assert.Equal(0, result.Content.TotalOrganizations);
        Assert.Equal(0, result.Content.TotalUsers);
    }

    [Fact]
    public async Task Handle_RepositoryCalledWithCorrectProjection()
    {
        // Arrange
        var query = new GetDashboardQuery();
        var dashboard = new DashboardDto();

        _repositoryMock.Setup(r => r.GetDashboardAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<
                    IQueryable<Domain.Entities.AggregateRoots.Organization>,
                    IQueryable<DashboardDto>>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(dashboard);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert - Verify projection is DashboardMappings.Projection
        _repositoryMock.Verify(r => r.GetDashboardAsync(
            DashboardMappings.Projection,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
