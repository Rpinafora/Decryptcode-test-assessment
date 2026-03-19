using Decryptcode.Assessment.Service.Application.Users.Dtos;
using Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;
using System.Linq.Expressions;

namespace Decryptcode.Assessment.Service.Application.Users.Mappings;

public static class UserMapping
{
    public static readonly Expression<Func<User, UserDto>> UserProjection = user => new UserDto
    {
        Id = user.Id,
        OrdId = user.OrgId,
        Email = user.Email,
        Name = user.Name,
        Role = user.Role,
        Active = user.Active,
        Bio = user.Bio
    };
}
