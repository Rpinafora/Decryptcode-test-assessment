using System.Net;

namespace Decryptcode.Assessment.Service.IntegrationTests.API;

public class InvoicesControllerIntegrationTests : ApiIntegrationTestBase
{
    [Fact]
    public async Task GetAllInvoices_ReturnsOkStatus()
    {
        // Act
        var response = await _client.GetAsync("/api/invoices");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllInvoices_ReturnsJsonContent()
    {
        // Act
        var response = await _client.GetAsync("/api/invoices");

        // Assert
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetAllInvoices_ReturnsValidContent()
    {
        // Act
        var response = await _client.GetAsync("/api/invoices");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.NotEmpty(content);
    }
}
