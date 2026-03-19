namespace Decryptcode.Assessment.Service.Application.TimeEntries.Dtos;

public sealed record TimeEntriesDto
{
    public required string Id { get; set; }

    public required string UserId { get; set; }

    public required string ProjectId { get; set; }

    public DateTime Date { get; set; }

    public int Hours { get; set; }

    public required string Description { get; set; }
}
