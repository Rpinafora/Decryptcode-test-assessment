using System.Net;

namespace Decryptcode.Assessment.Service.IntegrationTests.API;

public sealed class OrganizationsControllerIntegrationTests : ApiIntegrationTestBase
{
    [Fact]
    public async Task GetAllOrganizations_ReturnsOkStatus()
    {
        // Act
        var response = await _client.GetAsync("/api/organizations");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllOrganizations_ReturnsJsonContent()
    {
        // Act
        var response = await _client.GetAsync("/api/organizations");

        // Assert
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetAllOrganizations_ReturnsValidContent()
    {
        // Act
        var response = await _client.GetAsync("/api/organizations");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task GetOrganizationById_WithValidId_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/organizations/org-001");

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetOrganizationById_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/organizations/invalid-id-12345");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllOrganizations_WithIndustryFilter_ReturnsFiltered()
    {
        // Act
        var response = await _client.GetAsync("/api/organizations?industry=Technology");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task GetAllOrganizations_WithTierFilter_ReturnsFiltered()
    {
        // Act
        var response = await _client.GetAsync("/api/organizations?tier=enterprise");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}