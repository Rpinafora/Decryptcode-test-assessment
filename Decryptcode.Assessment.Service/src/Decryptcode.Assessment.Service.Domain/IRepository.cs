using Decryptcode.Assessment.Service.Domain.Entities;
using System.Linq.Expressions;

namespace Decryptcode.Assessment.Service.Domain;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    ValueTask<TEntity?> GetByIdAsync(string id, CancellationToken cancellationToken);

    ValueTask<TResult?> GetByIdAsync<TResult>(string id, Expression<Func<TEntity, TResult>>? projection = null, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken);

    Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
}
