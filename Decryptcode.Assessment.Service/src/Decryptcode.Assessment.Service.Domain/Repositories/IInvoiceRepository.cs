using Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;
using System.Linq.Expressions;

namespace Decryptcode.Assessment.Service.Domain.Repositories;

public interface IInvoiceRepository : IRepository<Invoice>
{
    public Task<IEnumerable<TResponse>> GetAllFiltered<TResponse>(
    string? orgId,
    string? status,
    Expression<Func<Invoice, TResponse>> projection,
    CancellationToken cancellationToken);
}
