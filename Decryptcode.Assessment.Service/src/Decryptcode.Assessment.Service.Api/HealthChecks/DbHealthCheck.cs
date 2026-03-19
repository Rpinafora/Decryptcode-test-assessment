using Decryptcode.Assessment.Service.Infrastructure.SqlServer.Contexts;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.Annotations;

namespace Decryptcode.Assessment.Service.Api.HealthChecks;

public class DbHealthCheck : IHealthCheck
{
    private readonly ApiContext _context;

    public DbHealthCheck(ApiContext context)
    {
        _context = context;
    }

    [SwaggerOperation(
     Summary = "Return the health situation of the API",
     Description = "Return the health situation of the API",
     Tags = ["Health"])
    ]
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
            return canConnect ? HealthCheckResult.Healthy("Database reachable") : HealthCheckResult.Unhealthy("Database unreachable");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database check failed", ex);
        }
    }
}
