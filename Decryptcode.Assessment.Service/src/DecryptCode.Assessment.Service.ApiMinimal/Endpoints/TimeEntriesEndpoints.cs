using Decryptcode.Assessment.Service.Application.TimeEntries;
using Decryptcode.Assessment.Service.Application.TimeEntries.Dtos;
using Wolverine;

namespace DecryptCode.Assessment.Service.ApiMinimal.Endpoints;

public static class TimeEntriesEndpoints
{
    public static WebApplication MapTimeEntriesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/time-entries")
            .WithTags("TimeEntries")
            .WithOpenApi();

        group.MapGet("/", GetTimeEntries)
            .WithName("GetTimeEntries")
            .WithDescription("Return all time entries with optional filters by user, project or date range")
            .WithOpenApi();

        return app;
    }

    private static async Task<IResult> GetTimeEntries(
        IMessageBus messageBus,
        string? userId,
        string? projectId,
        DateTime? from,
        DateTime? to,
        CancellationToken cancellationToken)
    {
        var query = new GetAllTimeEntriesQuery { UserId = userId, ProjectId = projectId, From = from, To = to };
        var result = await messageBus.InvokeAsync<dynamic>(query, cancellationToken);
        return Results.Ok(result);
    }
}
