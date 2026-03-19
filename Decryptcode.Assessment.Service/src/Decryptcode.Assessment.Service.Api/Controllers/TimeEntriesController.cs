using Decryptcode.Assessment.Service.Application.TimeEntries;
using Decryptcode.Assessment.Service.Application.TimeEntries.Dtos;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wolverine;

namespace Decryptcode.Assessment.Service.Api.Controllers;

[ApiController]
[Route("api/time-entries")]
[Produces("application/json")]
public sealed class TimeEntriesController : BaseController
{
    public TimeEntriesController(IMessageBus messageBus) : base(messageBus)
    {
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Return all time entries with optional filters",
        Description = "Return all time entries with optional filters by user, project or date range",
        Tags = ["TimeEntries"])
    ]
    [ProducesResponseType(typeof(List<TimeEntriesDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTimeEntriesAsync(
        [FromQuery] string? userId,
        [FromQuery] string? projectId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken)
    {
        var query = new GetAllTimeEntriesQuery { UserId = userId, ProjectId = projectId, From = from, To = to };

        return await SendAsync(query, cancellationToken);
    }
}
