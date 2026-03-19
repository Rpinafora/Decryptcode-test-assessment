using System.Net;

namespace Decryptcode.Assessment.Service.IntegrationTests.API;

public class HealthCheckIntegrationTests : ApiIntegrationTestBase
{
    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
