using Decryptcode.Assessment.Service.Application.Organizations.Dtos;
using Decryptcode.Assessment.Service.Application.Utils;

namespace Decryptcode.Assessment.Service.Application.Organizations.Queries.GetOrganizationSummary;

public sealed class GetOrganizationSummaryQuery : IRequest<OrganizationSummaryDto>
{
    public required string Id { get; set; }
}
