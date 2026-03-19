namespace DecryptCode.Assessment.Service.ApiMinimal.Endpoints;

public static class HealthEndpoints
{
    public static WebApplication MapHealthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/health")
            .WithTags("Health")
            .WithOpenApi();

        group.MapGet("/", GetHealth)
            .WithName("HealthCheck")
            .WithDescription("Health check endpoint")
            .WithOpenApi()
            .AllowAnonymous();

        return app;
    }

    private static IResult GetHealth()
    {
        return Results.Ok(new { status = "healthy" });
    }
}
