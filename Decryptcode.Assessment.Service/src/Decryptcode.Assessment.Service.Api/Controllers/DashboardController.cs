using Decryptcode.Assessment.Service.Application.Dashboard;
using Decryptcode.Assessment.Service.Application.Dashboard.Dtos;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wolverine;

namespace Decryptcode.Assessment.Service.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Produces("application/json")]
public sealed class DashboardController : BaseController
{
    public DashboardController(IMessageBus messageBus) : base(messageBus)
    {
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Return aggregated dashboard statistics",
        Description = "Return a set of aggregated counters across organizations, users, projects, time entries and invoices",
        Tags = new[] { "Dashboard" })
    ]
    [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardAsync(CancellationToken cancellationToken)
    {
        var query = new GetDashboardQuery();

        return await SendAsync(query, cancellationToken);
    }
}
