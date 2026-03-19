namespace Decryptcode.Assessment.Service.Application.Organizations.Dtos;

public sealed record OrganizationDto
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string Slug { get; init; }

    public required string Industry { get; init; }

    public required string Tier { get; init; }

    public required string ContactEmail { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required SettingsDto Settings { get; init; }

    public required MetadataDto Metadata { get; init; }
}
