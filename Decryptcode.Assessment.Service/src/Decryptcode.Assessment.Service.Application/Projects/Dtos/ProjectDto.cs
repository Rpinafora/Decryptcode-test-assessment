namespace Decryptcode.Assessment.Service.Application.Projects.Dtos;

public sealed record ProjectDto
{
    public required string Id { get; init; }

    public required string OrgId { get; init; }

    public required string Name { get; init; }

    public required string Status { get; set; }

    public int BudgetHours { get; init; }

    public DateOnly? StartDate { get; init; }

    public DateOnly? EndDate { get; init; }

    public required string Description { get; init; }
}
