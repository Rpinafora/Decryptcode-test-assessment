using Decryptcode.Assessment.Service.Application.Projects.Dtos;
using Decryptcode.Assessment.Service.Application.Projects.GetAllProjects;
using Decryptcode.Assessment.Service.Application.Projects.GetProjectById;
using Wolverine;

namespace DecryptCode.Assessment.Service.ApiMinimal.Endpoints;

public static class ProjectsEndpoints
{
    public static WebApplication MapProjectsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/projects")
            .WithTags("Projects")
            .WithOpenApi();

        group.MapGet("/", GetProjects)
            .WithName("GetProjects")
            .WithDescription("Return all projects with optional filters by organization or status")
            .WithOpenApi();

        group.MapGet("/{id}", GetProjectById)
            .WithName("GetProjectById")
            .WithDescription("Return the project by the Id specified")
            .WithOpenApi();

        return app;
    }

    private static async Task<IResult> GetProjects(
        IMessageBus messageBus,
        string? orgId,
        string? status,
        CancellationToken cancellationToken)
    {
        var query = new GetAllProjectsQuery { OrgId = orgId, Status = status };
        var result = await messageBus.InvokeAsync<dynamic>(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetProjectById(
        IMessageBus messageBus,
        string id,
        CancellationToken cancellationToken)
    {
        var query = new GetProjectByIdQuery { Id = id };
        var result = await messageBus.InvokeAsync<dynamic>(query, cancellationToken);
        return Results.Ok(result);
    }
}
