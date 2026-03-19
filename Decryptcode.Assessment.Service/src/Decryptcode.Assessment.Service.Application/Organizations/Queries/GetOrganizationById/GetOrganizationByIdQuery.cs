using Decryptcode.Assessment.Service.Application.Organizations.Dtos;
using Decryptcode.Assessment.Service.Application.Utils;

namespace Decryptcode.Assessment.Service.Application.Organizations.Queries.GetOrganizationById;

public sealed class GetOrganizationByIdQuery : IRequest<OrganizationDto>
{
    public required string Id { get; set; }
}
