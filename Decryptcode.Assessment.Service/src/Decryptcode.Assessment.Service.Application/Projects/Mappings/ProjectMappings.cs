using Decryptcode.Assessment.Service.Application.Organizations.Dtos;
using Decryptcode.Assessment.Service.Application.Projects.Dtos;
using Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;
using System.Linq.Expressions;

namespace Decryptcode.Assessment.Service.Application.Projects.Mappings;

public static class ProjectMappings
{
    public static readonly Expression<Func<Project, ProjectDto>> Projection = project =>
        new ProjectDto
        {
            Id = project.Id,
            OrgId = project.OrgId,
            Name = project.Name,
            BudgetHours = project.BudgetHours,
            StartDate = project.StartDate.HasValue ? DateOnly.FromDateTime(project.StartDate.Value) : null,
            EndDate = project.EndDate.HasValue ? DateOnly.FromDateTime(project.EndDate.Value) : null,
            Description = project.Description,
            Status = project.Status.ToString().ToLower(),
        };

    public static readonly Expression<Func<Project, ProjectSummaryDto>> SummaryProjection = project =>
        new ProjectSummaryDto
        {
            Name = project.Name,
            BudgetHours = project.BudgetHours,
            StartDate = project.StartDate.HasValue ? DateOnly.FromDateTime(project.StartDate.Value) : null,
            EndDate = project.EndDate.HasValue ? DateOnly.FromDateTime(project.EndDate.Value) : null,
            Status = project.Status.ToString().ToLower(),
            Organization = new OrganizationDto
            {
                Id = project.Organization!.Id,
                Name = project.Organization.Name,
                Slug = project.Organization.Slug,
                Industry = project.Organization.Industry,
                Tier = project.Organization.Tier,
                ContactEmail = project.Organization.ContactEmail,
                CreatedAt = project.Organization.CreatedAt,
                Settings = new SettingsDto(
                    project.Organization.Settings.Timezone,
                    project.Organization.Settings.Currency,
                    project.Organization.Settings.AllowOvertime,
                    project.Organization.Settings.DefaultLocale),
                Metadata = new MetadataDto(
                    project.Organization.Metadata.Source,
                    project.Organization.Metadata.LegacyId,
                    project.Organization.Metadata.MigratedAt)
            },

            TotalHoursLogged = project.TimeEntries.Sum(te => te.Hours)
        };
}
