using Decryptcode.Assessment.Service.Application.Utils.Results;

namespace Decryptcode.Assessment.Service.Application.Utils;

public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<IRequestResult<TResponse>> Handle(TRequest request, CancellationToken cancellationToken);
}
