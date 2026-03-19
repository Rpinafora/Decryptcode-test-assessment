namespace Decryptcode.Assessment.Service.Application.Organizations.Dtos;

public sealed record SettingsDto(string Timezone, string Currency, bool AllowOvertime, string DefaultLocale);
