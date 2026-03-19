using Decryptcode.Assessment.Service.Application.Organizations.Dtos;

namespace Decryptcode.Assessment.Service.Application.Projects.Dtos;

public sealed record ProjectSummaryDto
{
    public required OrganizationDto Organization { get; init; }

    public required string Name { get; init; }

    public required string Status { get; set; }

    public int BudgetHours { get; init; }

    public DateOnly? StartDate { get; init; }

    public DateOnly? EndDate { get; init; }

    public int TotalHoursLogged { get; init; }
}
