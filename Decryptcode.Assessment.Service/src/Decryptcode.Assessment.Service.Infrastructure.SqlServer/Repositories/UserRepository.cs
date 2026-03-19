using Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;
using Decryptcode.Assessment.Service.Domain.Repositories;
using Decryptcode.Assessment.Service.Infrastructure.SqlServer.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Decryptcode.Assessment.Service.Infrastructure.SqlServer.Repositories;

public sealed class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ApiContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TResponse>> GetAllFiltered<TResponse>(
        string? orgId,
        string? role,
        bool? active,
        Expression<Func<User, TResponse>> projection,
        CancellationToken cancellationToken)
    {
        var query = Query();

        if (!string.IsNullOrWhiteSpace(orgId))
        {
            query = query.Where(e => e.OrgId == orgId);
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            query = query.Where(e => e.Role == role);
        }

        if (active.HasValue)
        {
            query = query.Where(e => e.Active == active.Value);
        }

        return await query
            .Select(projection)
            .ToListAsync(cancellationToken);
    }
}
