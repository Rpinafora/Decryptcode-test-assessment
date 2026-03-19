using Decryptcode.Assessment.Service.Application.Users.Dtos;
using Decryptcode.Assessment.Service.Application.Users.Mappings;
using Decryptcode.Assessment.Service.Application.Utils;
using Decryptcode.Assessment.Service.Application.Utils.Results;
using Decryptcode.Assessment.Service.Domain.Repositories;

namespace Decryptcode.Assessment.Service.Application.Users.GetUserById;

public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IRequestResult<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, UserMapping.UserProjection, cancellationToken);

        if (user is null)
        {
            return RequestResultFactory<UserDto>.NotFound("User not found");
        }

        return RequestResultFactory<UserDto>.Ok(user);
    }
}
