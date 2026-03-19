using Decryptcode.Assessment.Service.Application.Users.Dtos;
using Decryptcode.Assessment.Service.Application.Utils;

namespace Decryptcode.Assessment.Service.Application.Users.GetAllUsers;

public sealed class GetAllUsersQuery : IRequest<IEnumerable<UserDto>>
{
    public string? OrgId { get; set; }

    public string? Role { get; set; }

    public bool? Active { get; set; }
}
