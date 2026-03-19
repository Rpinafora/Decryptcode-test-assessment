using Decryptcode.Assessment.Service.Application.Invoices.Dtos;
using Decryptcode.Assessment.Service.Application.Invoices.GetAllInvoices;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wolverine;

namespace Decryptcode.Assessment.Service.Api.Controllers;

[ApiController]
[Route("api/invoices")]
[Produces("application/json")]
public sealed class InvoicesController : BaseController
{
    public InvoicesController(IMessageBus messageBus) : base(messageBus)
    {
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Return all invoices with optional filters",
        Description = "Return all invoices with optional filters by organization or status",
        Tags = new[] { "Invoices" })
    ]
    [ProducesResponseType(typeof(List<InvoiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInvoicesAsync([FromQuery] string? orgId, [FromQuery] string? status, CancellationToken cancellationToken)
    {
        var query = new GetAllInvoicesQuery { OrgId = orgId, Status = status };

        return await SendAsync(query, cancellationToken);
    }
}
