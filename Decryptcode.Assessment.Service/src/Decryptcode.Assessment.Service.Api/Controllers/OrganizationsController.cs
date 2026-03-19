using Decryptcode.Assessment.Service.Application.Organizations.Dtos;
using Decryptcode.Assessment.Service.Application.Organizations.Queries.GetAllOrganizations;
using Decryptcode.Assessment.Service.Application.Organizations.Queries.GetOrganizationById;
using Decryptcode.Assessment.Service.Application.Organizations.Queries.GetOrganizationSummary;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wolverine;

namespace Decryptcode.Assessment.Service.Api.Controllers;

[ApiController]
[Route("api/organizations")]
[Produces("application/json")]
public sealed class OrganizationsController : BaseController
{

    public OrganizationsController(IMessageBus messageBus) : base(messageBus)
    {
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Return all Organizations available with the possibility to filtrate it by industry or tier",
        Description = "Return all Organizations available with the possibility to filtrate it by industry or tier",
        Tags = ["Organizations"])
    ]
    [ProducesResponseType(typeof(List<OrganizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganizationsAsync([FromQuery] string? industry, [FromQuery] string? tier, CancellationToken cancellationToken)
    {
        var query = new GetAllOrganizationsQuery { Industry = industry, Tier = tier };

        return await SendAsync(query, cancellationToken);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Return the organization by the Id specified",
        Description = "Return the organization by the Id specified",
        Tags = ["Organizations"])
    ]
    [ProducesResponseType(typeof(List<OrganizationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<OrganizationDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrganizationByIdAsync([FromRoute] string id, CancellationToken cancellationToken)
    {
        var query = new GetOrganizationByIdQuery { Id = id };

        return await SendAsync(query, cancellationToken);
    }

    [HttpGet("{id}/summary")]
    [SwaggerOperation(
     Summary = "Return the organization by the Id specified",
     Description = "Return the organization by the Id specified",
     Tags = ["Organizations"])
    ]
    [ProducesResponseType(typeof(List<OrganizationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<OrganizationDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrganizationSummaryAsync([FromRoute] string id, CancellationToken cancellationToken)
    {
        var query = new GetOrganizationSummaryQuery { Id = id };

        return await SendAsync(query, cancellationToken);
    }
}