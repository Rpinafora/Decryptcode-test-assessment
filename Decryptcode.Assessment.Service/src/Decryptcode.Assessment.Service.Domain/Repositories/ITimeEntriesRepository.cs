using Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;
using System.Linq.Expressions;

namespace Decryptcode.Assessment.Service.Domain.Repositories;

public interface ITimeEntriesRepository : IRepository<TimeEntry>
{
    public Task<IEnumerable<TResponse>> GetAllFiltered<TResponse>(
        string? userId,
        string? projectId,
        DateTime? from,
        DateTime? to,
        Expression<Func<TimeEntry, TResponse>> projection,
        CancellationToken cancellationToken);
}
