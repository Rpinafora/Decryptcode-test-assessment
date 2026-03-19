using Decryptcode.Assessment.Service.Application.Invoices.Dtos;
using Decryptcode.Assessment.Service.Application.Invoices.GetAllInvoices;
using Wolverine;

namespace DecryptCode.Assessment.Service.ApiMinimal.Endpoints;

public static class InvoicesEndpoints
{
    public static WebApplication MapInvoicesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/invoices")
            .WithTags("Invoices")
            .WithOpenApi();

        group.MapGet("/", GetInvoices)
            .WithName("GetInvoices")
            .WithDescription("Return all invoices with optional filters by organization or status")
            .WithOpenApi();

        return app;
    }

    private static async Task<IResult> GetInvoices(
        IMessageBus messageBus,
        string? orgId,
        string? status,
        CancellationToken cancellationToken)
    {
        var query = new GetAllInvoicesQuery { OrgId = orgId, Status = status };
        var result = await messageBus.InvokeAsync<dynamic>(query, cancellationToken);
        return Results.Ok(result);
    }
}
