using Decryptcode.Assessment.Service.Application.Dashboard;
using Decryptcode.Assessment.Service.Application.Dashboard.Dtos;
using Wolverine;

namespace DecryptCode.Assessment.Service.ApiMinimal.Endpoints;

public static class DashboardEndpoints
{
    public static WebApplication MapDashboardEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/dashboard")
            .WithTags("Dashboard")
            .WithOpenApi();

        group.MapGet("/", GetDashboard)
            .WithName("GetDashboard")
            .WithDescription("Return aggregated dashboard statistics across organizations, users, projects, time entries and invoices")
            .WithOpenApi();

        return app;
    }

    private static async Task<IResult> GetDashboard(
        IMessageBus messageBus,
        CancellationToken cancellationToken)
    {
        var query = new GetDashboardQuery();
        var result = await messageBus.InvokeAsync<dynamic>(query, cancellationToken);
        return Results.Ok(result);
    }
}
