using Decryptcode.Assessment.Service.Application.Users.Dtos;
using Decryptcode.Assessment.Service.Application.Users.GetAllUsers;
using Decryptcode.Assessment.Service.Application.Users.GetUserById;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wolverine;

namespace Decryptcode.Assessment.Service.Api.Controllers;

[ApiController]
[Route("api/users")]
[Produces("application/json")]
public sealed class UsersController : BaseController
{

    public UsersController(IMessageBus messageBus) : base(messageBus)
    {
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Return all Users available with the possibility to filtrate it by organization or Role or Active status",
        Description = "Return all Users available with the possibility to filtrate it by organization or Role or Active status",
        Tags = ["Users"])
    ]
    [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsersAsync([FromQuery] string? ordId, [FromQuery] string? role, [FromQuery] bool? active, CancellationToken cancellationToken)
    {
        var query = new GetAllUsersQuery { OrgId = ordId, Role = role, Active = active };

        return await SendAsync(query, cancellationToken);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Return the Users by the Id specified",
        Description = "Return the Users by the Id specified",
        Tags = ["Users"])
    ]
    [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserByIdAsync([FromRoute] string id, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery { Id = id };

        return await SendAsync(query, cancellationToken);
    }
}