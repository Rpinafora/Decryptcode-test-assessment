using Decryptcode.Assessment.Service.Application.Organizations.Dtos;
using Decryptcode.Assessment.Service.Application.Utils;

namespace Decryptcode.Assessment.Service.Application.Organizations.Queries.GetAllOrganizations;

public sealed class GetAllOrganizationsQuery : IRequest<IEnumerable<OrganizationDto>>
{
    public string? Industry { get; set; } = string.Empty;

    public string? Tier { get; set; } = string.Empty;
}
