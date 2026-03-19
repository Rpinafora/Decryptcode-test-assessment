using Decryptcode.Assessment.Service.Application.Organizations.Dtos;
using Decryptcode.Assessment.Service.Application.Organizations.Mappings;
using Decryptcode.Assessment.Service.Application.Utils;
using Decryptcode.Assessment.Service.Application.Utils.Results;
using Decryptcode.Assessment.Service.Domain.Repositories;

namespace Decryptcode.Assessment.Service.Application.Organizations.Queries.GetOrganizationById;

public sealed class GetOrganizationByIdQueryHandler : IRequestHandler<GetOrganizationByIdQuery, OrganizationDto>
{
    private readonly IOrganizationRepository _organizationRepository;

    public GetOrganizationByIdQueryHandler(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<IRequestResult<OrganizationDto>> Handle(GetOrganizationByIdQuery request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetByIdAsync(request.Id, cancellationToken);

        if (organization is null)
        {
            return RequestResultFactory<OrganizationDto>.NotFound("Organization not found");
        }

        return RequestResultFactory<OrganizationDto>.Ok(organization.ToDto());
    }
}
