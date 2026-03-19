using Decryptcode.Assessment.Service.Domain.Entities.AggregateRoots;
using System.Linq.Expressions;

namespace Decryptcode.Assessment.Service.Domain.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    public Task<IEnumerable<Organization>> GetAllFiltered(string? industry, string? tier, CancellationToken cancellationToken);

    public Task<TResponse?> GetDashboardAsync<TResponse>(Expression<Func<IQueryable<Organization>, IQueryable<TResponse>>> projection, CancellationToken cancellationToken);
}
