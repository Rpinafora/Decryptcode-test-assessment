using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;

namespace Decryptcode.Assessment.Service.IntegrationTests.API;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseContentRoot(Path.Combine(Directory.GetCurrentDirectory(), ".."));
        base.ConfigureWebHost(builder);
    }
}

public abstract class ApiIntegrationTestBase : IAsyncLifetime
{
    protected readonly WebApplicationFactory<Program> _factory;
    protected readonly HttpClient _client;
    protected readonly JsonSerializerOptions _jsonOptions;

    public ApiIntegrationTestBase()
    {
        // Set connection string as environment variable BEFORE factory creation
        // This ensures it's available during Program.cs configuration building
        var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DecryptCodeAssessmentTest;Integrated Security=True;Trust Server Certificate=True";
        Environment.SetEnvironmentVariable("ConnectionStrings__SqlConnectionString", connectionString);

        _factory = new TestWebApplicationFactory();
        _client = _factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public virtual async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    public virtual async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    protected T? DeserializeJsonResponse<T>(string content)
    {
        return JsonSerializer.Deserialize<T>(content, _jsonOptions);
    }

    protected string SerializeToJson<T>(T obj)
    {
        return JsonSerializer.Serialize(obj);
    }
}