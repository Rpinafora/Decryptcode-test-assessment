using Microsoft.AspNetCore.Http;

namespace Decryptcode.Assessment.Service.Infrastructure.SqlServer;

public partial class DesignTimeDbContextFactory
{
    private sealed class DesignTimeHttpContextAccessor : IHttpContextAccessor
    {
        public HttpContext? HttpContext { get; set; }
    }
}
