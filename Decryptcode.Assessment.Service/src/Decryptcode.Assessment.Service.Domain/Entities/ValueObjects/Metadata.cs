using Decryptcode.Assessment.Service.Domain.Guards;

namespace Decryptcode.Assessment.Service.Domain.Entities.ValueObjects;

public sealed record Metadata
{
    public string Source { get; init; }
    public int? LegacyId { get; init; }
    public DateTime? MigratedAt { get; init; }

    public Metadata(string source, int? legacyId, DateTime? migratedAt)
    {
        Guard.NullOrWhiteSpace(source, nameof(source));
        Source = source;
        LegacyId = legacyId;
        MigratedAt = migratedAt;
    }
}