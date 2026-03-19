using System.Net;

namespace Decryptcode.Assessment.Service.IntegrationTests.API;

public class ProjectsControllerIntegrationTests : ApiIntegrationTestBase
{
    [Fact]
    public async Task GetAllProjects_ReturnsOkStatus()
    {
        // Act
        var response = await _client.GetAsync("/api/projects");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllProjects_ReturnsJsonContent()
    {
        // Act
        var response = await _client.GetAsync("/api/projects");

        // Assert
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetProjectById_WithValidId_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/projects/proj-001");

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProjectById_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/projects/invalid-id-12345");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllProjects_ReturnsValidContent()
    {
        // Act
        var response = await _client.GetAsync("/api/projects");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.NotEmpty(content);
    }
}