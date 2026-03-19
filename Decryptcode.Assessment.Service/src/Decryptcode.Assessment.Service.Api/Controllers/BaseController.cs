using Decryptcode.Assessment.Service.Application.Utils.Results;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Decryptcode.Assessment.Service.Api.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected readonly IMessageBus _messageBus;

    protected BaseController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    protected async Task<IActionResult> SendAsync(object message, CancellationToken cancellationToken)
    {
        var result = await _messageBus.InvokeAsync<IRequestResult>(message, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }
}
