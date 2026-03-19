using Decryptcode.Assessment.Service.Application.Organizations.Dtos;
using Decryptcode.Assessment.Service.Domain.Entities.AggregateRoots;
using System.Linq.Expressions;

namespace Decryptcode.Assessment.Service.Application.Organizations.Mappings;

public static class OrganizationMappings
{
    public static OrganizationDto ToDto(this Organization organization)
    {
        ArgumentNullException.ThrowIfNull(organization);

        return new OrganizationDto
        {
            Id = organization.Id,
            Name = organization.Name,
            Slug = organization.Slug,
            Industry = organization.Industry,
            Tier = organization.Tier,
            ContactEmail = organization.ContactEmail,
            CreatedAt = organization.CreatedAt,
            Settings = new SettingsDto(
                organization.Settings.Timezone,
                organization.Settings.Currency,
                organization.Settings.AllowOvertime,
                organization.Settings.DefaultLocale),
            Metadata = new MetadataDto(
                organization.Metadata.Source,
                organization.Metadata.LegacyId,
                organization.Metadata.MigratedAt)
        };
    }

    public static readonly Expression<Func<Organization, OrganizationDto>> Projection = organization =>
        new OrganizationDto
        {
            Id = organization.Id,
            Name = organization.Name,
            Slug = organization.Slug,
            Industry = organization.Industry,
            Tier = organization.Tier,
            ContactEmail = organization.ContactEmail,
            CreatedAt = organization.CreatedAt,
            Settings = new SettingsDto(
                organization.Settings.Timezone,
                organization.Settings.Currency,
                organization.Settings.AllowOvertime,
                organization.Settings.DefaultLocale),
            Metadata = new MetadataDto(
                organization.Metadata.Source,
                organization.Metadata.LegacyId,
                organization.Metadata.MigratedAt)
        };

    public static readonly Expression<Func<Organization, OrganizationSummaryDto>> SummaryProjection = organization =>
        new OrganizationSummaryDto
        {
            Organization = new OrganizationDto
            {
                Id = organization.Id,
                Name = organization.Name,
                Slug = organization.Slug,
                Industry = organization.Industry,
                Tier = organization.Tier,
                ContactEmail = organization.ContactEmail,
                CreatedAt = organization.CreatedAt,
                Settings = new SettingsDto(
                    organization.Settings.Timezone,
                    organization.Settings.Currency,
                    organization.Settings.AllowOvertime,
                    organization.Settings.DefaultLocale),
                Metadata = new MetadataDto(
                    organization.Metadata.Source,
                    organization.Metadata.LegacyId,
                    organization.Metadata.MigratedAt)
            },
            ProjectCount = organization.Projects != null ? organization.Projects.Count() : 0,
            UserCount = organization.Users != null ? organization.Users.Count() : 0,
            TotalInvoiced = organization.Invoices != null ? organization.Invoices.Sum(e => e.Amount) : 0,
            Currency = organization.Settings != null ? organization.Settings.Currency : "USD"
        };

    public static IEnumerable<OrganizationDto> ToDtos(this IEnumerable<Organization> organizations) =>
        organizations?.Select(o => o.ToDto()) ?? [];
}
