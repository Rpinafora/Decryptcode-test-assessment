using Decryptcode.Assessment.Service.Application.Users.Dtos;
using Decryptcode.Assessment.Service.Application.Utils;

namespace Decryptcode.Assessment.Service.Application.Users.GetUserById;

public sealed class GetUserByIdQuery : IRequest<UserDto>
{
    public required string Id { get; set; }
}
