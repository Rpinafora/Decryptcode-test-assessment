using Decryptcode.Assessment.Service.Application.Organizations.Dtos;
using Decryptcode.Assessment.Service.Application.Organizations.Queries.GetAllOrganizations;
using Decryptcode.Assessment.Service.Application.Organizations.Queries.GetOrganizationById;
using Decryptcode.Assessment.Service.Application.Organizations.Queries.GetOrganizationSummary;
using Wolverine;

namespace DecryptCode.Assessment.Service.ApiMinimal.Endpoints;

public static class OrganizationsEndpoints
{
    public static WebApplication MapOrganizationsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/organizations")
            .WithTags("Organizations")
            .WithOpenApi();

        group.MapGet("/", GetOrganizations)
            .WithName("GetOrganizations")
            .WithDescription("Return all Organizations available with the possibility to filtrate it by industry or tier")
            .WithOpenApi();

        group.MapGet("/{id}", GetOrganizationById)
            .WithName("GetOrganizationById")
            .WithDescription("Return the organization by the Id specified")
            .WithOpenApi();

        group.MapGet("/{id}/summary", GetOrganizationSummary)
            .WithName("GetOrganizationSummary")
            .WithDescription("Return the organization summary by the Id specified")
            .WithOpenApi();

        return app;
    }

    private static async Task<IResult> GetOrganizations(
        IMessageBus messageBus,
        string? industry,
        string? tier,
        CancellationToken cancellationToken)
    {
        var query = new GetAllOrganizationsQuery { Industry = industry, Tier = tier };
        var result = await messageBus.InvokeAsync<dynamic>(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetOrganizationById(
        IMessageBus messageBus,
        string id,
        CancellationToken cancellationToken)
    {
        var query = new GetOrganizationByIdQuery { Id = id };
        var result = await messageBus.InvokeAsync<dynamic>(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetOrganizationSummary(
        IMessageBus messageBus,
        string id,
        CancellationToken cancellationToken)
    {
        var query = new GetOrganizationSummaryQuery { Id = id };
        var result = await messageBus.InvokeAsync<dynamic>(query, cancellationToken);
        return Results.Ok(result);
    }
}

