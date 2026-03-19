namespace Decryptcode.Assessment.Service.Application.Organizations.Dtos;

public sealed class OrganizationSummaryDto
{
    public required OrganizationDto Organization { get; init; }

    public int ProjectCount { get; init; }

    public int UserCount { get; init; }

    public decimal TotalInvoiced { get; init; }

    public required string Currency { get; init; }
}
