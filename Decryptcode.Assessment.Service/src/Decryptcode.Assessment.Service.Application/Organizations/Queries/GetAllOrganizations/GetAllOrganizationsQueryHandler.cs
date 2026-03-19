using Decryptcode.Assessment.Service.Application.Organizations.Dtos;
using Decryptcode.Assessment.Service.Application.Organizations.Mappings;
using Decryptcode.Assessment.Service.Application.Utils;
using Decryptcode.Assessment.Service.Application.Utils.Results;
using Decryptcode.Assessment.Service.Domain.Repositories;

namespace Decryptcode.Assessment.Service.Application.Organizations.Queries.GetAllOrganizations;

public sealed class GetAllOrganizationsQueryHandler : IRequestHandler<GetAllOrganizationsQuery, IEnumerable<OrganizationDto>>
{
    private readonly IOrganizationRepository _organizationRepository;

    public GetAllOrganizationsQueryHandler(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<IRequestResult<IEnumerable<OrganizationDto>>> Handle(GetAllOrganizationsQuery request, CancellationToken cancellationToken)
    {
        var organizations = await _organizationRepository
            .GetAllFiltered(request.Industry, request.Tier, cancellationToken);

        return RequestResultFactory<IEnumerable<OrganizationDto>>.Ok(organizations.ToDtos());
    }
}
