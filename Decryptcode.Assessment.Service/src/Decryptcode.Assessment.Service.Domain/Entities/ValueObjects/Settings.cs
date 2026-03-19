using Decryptcode.Assessment.Service.Domain.Guards;
using Microsoft.EntityFrameworkCore;

namespace Decryptcode.Assessment.Service.Domain.Entities.ValueObjects;

[Owned]
public sealed record Settings
{
    public string Timezone { get; init; }
    public string Currency { get; init; }
    public bool AllowOvertime { get; init; }
    public string DefaultLocale { get; init; }

    public Settings(string timezone, string currency, bool allowOvertime, string defaultLocale)
    {
        Guard.NullOrWhiteSpace(timezone, nameof(timezone));
        Guard.NullOrWhiteSpace(currency, nameof(currency));
        Guard.NullOrWhiteSpace(defaultLocale, nameof(defaultLocale));
        Timezone = timezone;
        Currency = currency;
        AllowOvertime = allowOvertime;
        DefaultLocale = defaultLocale;
    }
}