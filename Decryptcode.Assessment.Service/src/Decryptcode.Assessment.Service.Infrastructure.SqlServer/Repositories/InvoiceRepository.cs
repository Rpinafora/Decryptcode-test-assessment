using Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;
using Decryptcode.Assessment.Service.Domain.Enums;
using Decryptcode.Assessment.Service.Domain.Repositories;
using Decryptcode.Assessment.Service.Infrastructure.SqlServer.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Decryptcode.Assessment.Service.Infrastructure.SqlServer.Repositories;

public sealed class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(ApiContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TResponse>> GetAllFiltered<TResponse>(
        string? orgId,
        string? status,
        Expression<Func<Invoice, TResponse>> projection,
        CancellationToken cancellationToken)
    {
        var query = Query();

        if (!string.IsNullOrWhiteSpace(orgId))
        {
            query = query.Where(e => e.OrgId == orgId);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (Enum.TryParse<InvoiceStatus>(status, ignoreCase: true, out var parsed))
            {
                query = query.Where(e => e.Status == parsed);
            }
        }

        return await query
            .Select(projection)
            .ToListAsync(cancellationToken);
    }
}
