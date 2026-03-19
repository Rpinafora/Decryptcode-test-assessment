using Decryptcode.Assessment.Service.Domain;
using Decryptcode.Assessment.Service.Domain.Entities;
using Decryptcode.Assessment.Service.Infrastructure.SqlServer.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Decryptcode.Assessment.Service.Infrastructure.SqlServer.Repositories;

public class BaseRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly DbSet<T> _dbSet;
    protected readonly ApiContext _context;

    public BaseRepository(ApiContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IQueryable<T> Query(bool asNoTracking = true) =>
        asNoTracking ? _dbSet.AsNoTracking() : _dbSet;

    public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await Query()
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await Query().ToListAsync(cancellationToken);
    }

    public async ValueTask<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async ValueTask<TResult?> GetByIdAsync<TResult>(string id, Expression<Func<T, TResult>>? projection, CancellationToken cancellationToken = default)
    {
        if (projection == null) throw new ArgumentNullException(nameof(projection));

        return await Query()
            .Where(e => e.Id == id)
            .Select(projection)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
