using Decryptcode.Assessment.Service.Application.Dashboard.Dtos;
using Decryptcode.Assessment.Service.Application.Dashboard.Mappings;
using Decryptcode.Assessment.Service.Application.Utils;
using Decryptcode.Assessment.Service.Application.Utils.Results;
using Decryptcode.Assessment.Service.Domain.Repositories;

namespace Decryptcode.Assessment.Service.Application.Dashboard;

public sealed class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    private readonly IOrganizationRepository _organizationRepository;

    public GetDashboardQueryHandler(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<IRequestResult<DashboardDto>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var dashboard = await _organizationRepository.GetDashboardAsync(DashboardMappings.Projection, cancellationToken);

        return RequestResultFactory<DashboardDto>.Ok(dashboard!);
    }
}
