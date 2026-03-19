using Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;
using Decryptcode.Assessment.Service.Domain.Repositories;
using Decryptcode.Assessment.Service.Infrastructure.SqlServer.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Decryptcode.Assessment.Service.Infrastructure.SqlServer.Repositories;

public sealed class TimeEntryRepository : BaseRepository<TimeEntry>, ITimeEntriesRepository
{
    public TimeEntryRepository(ApiContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TResponse>> GetAllFiltered<TResponse>(
        string? userId,
        string? projectId,
        DateTime? from,
        DateTime? to,
        Expression<Func<TimeEntry, TResponse>> projection,
        CancellationToken cancellationToken)
    {
        var query = Query();

        if (!string.IsNullOrWhiteSpace(userId))
        {
            query = query.Where(e => e.UserId == userId);
        }

        if (!string.IsNullOrWhiteSpace(projectId))
        {
            query = query.Where(e => e.ProjectId == projectId);
        }

        if (from.HasValue)
        {
            query = query.Where(e => e.Date >= from);
        }

        if (to.HasValue)
        {
            query = query.Where((e) => e.Date <= to);
        }

        return await query
            .Select(projection)
            .ToListAsync(cancellationToken);
    }
}
