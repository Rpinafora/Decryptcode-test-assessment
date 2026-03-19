namespace Decryptcode.Assessment.Service.Application.Dashboard.Dtos;

public sealed record DashboardDto
{
    public int TotalOrganizations { get; init; }

    public int TotalUsers { get; init; }

    public int TotalProjects { get; init; }

    public int ActiveProjects { get; init; }

    public int TotalTimeEntries { get; init; }

    public decimal TotalInvoiced { get; init; }
}
