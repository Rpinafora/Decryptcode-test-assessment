using Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;
using System.Linq.Expressions;

namespace Decryptcode.Assessment.Service.Domain.Repositories;

public interface IProjectRepository : IRepository<Project>
{
    public Task<IEnumerable<TResponse>> GetAllFiltered<TResponse>(string? orgId, string? status, Expression<Func<Project, TResponse>> projection, CancellationToken cancellationToken);
}
