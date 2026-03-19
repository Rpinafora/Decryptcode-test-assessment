using Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;
using System.Linq.Expressions;

namespace Decryptcode.Assessment.Service.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    public Task<IEnumerable<TResponse>> GetAllFiltered<TResponse>(
        string? orgId,
        string? role,
        bool? active,
        Expression<Func<User, TResponse>> projection,
        CancellationToken cancellationToken);
}
