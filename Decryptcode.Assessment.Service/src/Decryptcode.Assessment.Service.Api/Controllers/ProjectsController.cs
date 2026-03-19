using Decryptcode.Assessment.Service.Application.Projects.Dtos;
using Decryptcode.Assessment.Service.Application.Projects.GetAllProjects;
using Decryptcode.Assessment.Service.Application.Projects.GetProjectById;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wolverine;

namespace Decryptcode.Assessment.Service.Api.Controllers;

[ApiController]
[Route("api/projects")]
[Produces("application/json")]
public sealed class ProjectsController : BaseController
{

    public ProjectsController(IMessageBus messageBus) : base(messageBus)
    {
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Return all Projects available with the possibility to filtrate it by organization or status",
        Description = "Return all Projects available with the possibility to filtrate it by organization or status",
        Tags = ["Projects"])
    ]
    [ProducesResponseType(typeof(List<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjectsAsync([FromQuery] string? orgId, [FromQuery] string? status, CancellationToken cancellationToken)
    {
        var query = new GetAllProjectsQuery { OrgId = orgId, Status = status };

        return await SendAsync(query, cancellationToken);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Return the Projects by the Id specified",
        Description = "Return the Projects by the Id specified",
        Tags = ["Projects"])
    ]
    [ProducesResponseType(typeof(List<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<ProjectDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProjectByIdAsync([FromRoute] string id, CancellationToken cancellationToken)
    {
        var query = new GetProjectByIdQuery { Id = id };

        return await SendAsync(query, cancellationToken);
    }
}