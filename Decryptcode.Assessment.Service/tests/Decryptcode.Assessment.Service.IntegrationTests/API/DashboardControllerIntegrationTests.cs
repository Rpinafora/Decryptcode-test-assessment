using System.Net;

namespace Decryptcode.Assessment.Service.IntegrationTests.API;

public sealed class DashboardControllerIntegrationTests : ApiIntegrationTestBase
{
    [Fact]
    public async Task GetDashboard_ReturnsOkStatus()
    {
        // Act
        var response = await _client.GetAsync("/api/dashboard");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetDashboard_ReturnsValidContent()
    {
        // Act
        var response = await _client.GetAsync("/api/dashboard");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.NotEmpty(content);
        Assert.Contains("totalOrganizations", content);
        Assert.Contains("totalUsers", content);
        Assert.Contains("totalProjects", content);
    }

    [Fact]
    public async Task GetDashboard_ReturnsJsonContent()
    {
        // Act
        var response = await _client.GetAsync("/api/dashboard");

        // Assert
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetDashboard_ReturnsValidDashboardDto()
    {
        // Act
        var response = await _client.GetAsync("/api/dashboard");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.NotNull(content);
        Assert.Contains("totalOrganizations", content);
        Assert.Contains("totalUsers", content);
        Assert.Contains("totalProjects", content);
    }
}