using Decryptcode.Assessment.Service.Application.Organizations.Dtos;
using Decryptcode.Assessment.Service.Application.Organizations.Mappings;
using Decryptcode.Assessment.Service.Application.Organizations.Queries.GetOrganizationSummary;
using Decryptcode.Assessment.Service.Application.Utils;
using Decryptcode.Assessment.Service.Application.Utils.Results;
using Decryptcode.Assessment.Service.Domain.Repositories;

namespace Decryptcode.Assessment.Service.Application.Organizations.Queries.GetOrganizationSumarry;

public sealed class GetOrganizationSummaryHandler : IRequestHandler<GetOrganizationSummaryQuery, OrganizationSummaryDto>
{
    private readonly IOrganizationRepository _organizationRepository;

    public GetOrganizationSummaryHandler(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<IRequestResult<OrganizationSummaryDto>> Handle(GetOrganizationSummaryQuery request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetByIdAsync(request.Id, OrganizationMappings.SummaryProjection, cancellationToken);

        if (organization is null)
        {
            return RequestResultFactory<OrganizationSummaryDto>.NotFound("Organization not found");
        }

        return RequestResultFactory<OrganizationSummaryDto>.Ok(organization);
    }
}
