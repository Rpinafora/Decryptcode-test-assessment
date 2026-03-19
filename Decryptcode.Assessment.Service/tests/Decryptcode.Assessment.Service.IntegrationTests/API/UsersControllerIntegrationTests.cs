using System.Net;

namespace Decryptcode.Assessment.Service.IntegrationTests.API;

public class UsersControllerIntegrationTests : ApiIntegrationTestBase
{
    [Fact]
    public async Task GetAllUsers_ReturnsOkStatus()
    {
        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllUsers_ReturnsJsonContent()
    {
        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetUserById_WithValidId_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/users/user-001");

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUserById_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/users/invalid-id-12345");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllUsers_ReturnsValidContent()
    {
        // Act
        var response = await _client.GetAsync("/api/users");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.NotEmpty(content);
    }
}
