using Decryptcode.Assessment.Service.Domain.Entities.AggregateRoots;
using Decryptcode.Assessment.Service.Domain.Repositories;
using Decryptcode.Assessment.Service.Infrastructure.SqlServer.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Decryptcode.Assessment.Service.Infrastructure.SqlServer.Repositories;

public class OrganizationRepository : BaseRepository<Organization>, IOrganizationRepository
{
    public OrganizationRepository(ApiContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Organization>> GetAllFiltered(string? industry, string? tier, CancellationToken cancellationToken)
    {
        var query = Query();

        if (!string.IsNullOrWhiteSpace(tier))
        {
            query = query.Where(o => o.Tier == tier);
        }

        if (!string.IsNullOrWhiteSpace(industry))
        {
            query = query.Where(o => o.Industry == industry);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TResponse?> GetDashboardAsync<TResponse>(Expression<Func<IQueryable<Organization>, IQueryable<TResponse>>> projection, CancellationToken cancellationToken)
    {
        var query = Query()
            .Include(o => o.Users)
            .Include(o => o.Projects)
                .ThenInclude(p => p.TimeEntries)
            .Include(o => o.Invoices);


        try
        {
            var invoked = Expression.Invoke(projection, Expression.Constant(query, typeof(IQueryable<Organization>)));
            var projectedQueryable = query.Provider.CreateQuery<TResponse>(invoked);

            return await projectedQueryable.FirstOrDefaultAsync(cancellationToken);
        }
        catch (InvalidOperationException)
        {
            var list = await query.ToListAsync(cancellationToken);

            var compiled = projection.Compile();
            var resultQueryable = compiled(list.AsQueryable());

            return resultQueryable.FirstOrDefault();
        }
    }
}
