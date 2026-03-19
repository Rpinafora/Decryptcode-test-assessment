using Decryptcode.Assessment.Service.Application.Users.Dtos;
using Decryptcode.Assessment.Service.Application.Users.Mappings;
using Decryptcode.Assessment.Service.Application.Utils;
using Decryptcode.Assessment.Service.Application.Utils.Results;
using Decryptcode.Assessment.Service.Domain.Repositories;

namespace Decryptcode.Assessment.Service.Application.Users.GetAllUsers;

public sealed class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IRequestResult<IEnumerable<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllFiltered(request.OrgId, request.Role, request.Active, UserMapping.UserProjection, cancellationToken);

        return RequestResultFactory<IEnumerable<UserDto>>.Ok(users);
    }
}
