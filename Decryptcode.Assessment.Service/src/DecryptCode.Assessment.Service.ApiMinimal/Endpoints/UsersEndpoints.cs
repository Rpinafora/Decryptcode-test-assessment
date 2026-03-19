using Decryptcode.Assessment.Service.Application.Users.GetAllUsers;
using Decryptcode.Assessment.Service.Application.Users.GetUserById;
using Wolverine;

namespace DecryptCode.Assessment.Service.ApiMinimal.Endpoints;

public static class UsersEndpoints
{
    public static WebApplication MapUsersEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users")
            .WithOpenApi();

        group.MapGet("/", GetUsers)
            .WithName("GetUsers")
            .WithDescription("Return all users with optional filters by organization, role or active status")
            .WithOpenApi();

        group.MapGet("/{id}", GetUserById)
            .WithName("GetUserById")
            .WithDescription("Return the user by the Id specified")
            .WithOpenApi();

        return app;
    }

    private static async Task<IResult> GetUsers(
        IMessageBus messageBus,
        string? ordId,
        string? role,
        bool? active,
        CancellationToken cancellationToken)
    {
        var query = new GetAllUsersQuery { OrgId = ordId, Role = role, Active = active };
        var result = await messageBus.InvokeAsync<dynamic>(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetUserById(
        IMessageBus messageBus,
        string id,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery { Id = id };
        var result = await messageBus.InvokeAsync<dynamic>(query, cancellationToken);
        return Results.Ok(result);
    }
}
