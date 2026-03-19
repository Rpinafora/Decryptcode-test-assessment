using Decryptcode.Assessment.Service.Domain.Entities.AggregateRoots;
using Decryptcode.Assessment.Service.Domain.Enums;
using Decryptcode.Assessment.Service.Domain.Guards;
using System.Diagnostics.CodeAnalysis;

namespace Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;

public sealed class Project : BaseEntity
{
    public string OrgId { get; private set; }

    public string Name { get; private set; }

    // This status should be a status Id or a enum with the status. Using like a string can lead to a few wrong behaviors.
    public ProjectStatus Status { get; private set; }

    public int BudgetHours { get; private set; }

    public DateTime? StartDate { get; private set; }

    public DateTime? EndDate { get; private set; }

    public string Description { get; private set; }

    public Organization? Organization { get; private set; }

    public ICollection<Invoice> Invoices { get; private set; } = new HashSet<Invoice>();

    public ICollection<TimeEntry> TimeEntries { get; private set; } = new HashSet<TimeEntry>();

    private Project()
    {
        // For EF Core
    }

    [SetsRequiredMembers]
    private Project(
        string id,
        string orgId,
        string name,
        ProjectStatus status,
        int budgetHours,
        DateTime? startDate,
        DateTime? endDate,
        string description)
    {
        Guard.NullOrEmpty(name);
        Guard.NullOrEmpty(description);
        Guard.OutOfRange(budgetHours, 0, int.MaxValue, nameof(budgetHours));

        Id = id;
        OrgId = orgId;
        Name = name;
        Status = status;
        BudgetHours = budgetHours;
        StartDate = startDate;
        EndDate = endDate;
        Description = description;
    }

    public static Project Create(
        string id,
        string orgId,
        string name,
        ProjectStatus status,
        int budgetHours,
        DateTime? startDate,
        DateTime? endDate,
        string description
        )
    {
        return new Project(id, orgId, name, status, budgetHours, startDate, endDate, description);
    }

    public void ChangeName(string name)
    {
        Guard.NullOrWhiteSpace(name, nameof(name));
        Name = name;
    }

    public void Activate()
    {
        Status = ProjectStatus.Active;
    }

    public void Complete()
    {
        Status = ProjectStatus.Completed;
    }

    public void Plan()
    {
        Status = ProjectStatus.Planned;
    }

    public void ChangeBudgetHours(int budgetHours)
    {
        Guard.OutOfRange(budgetHours, 0, int.MaxValue, nameof(budgetHours));
        BudgetHours = budgetHours;
    }

    public void ChangeStartDate(DateTime startDate)
    {
        StartDate = startDate;
    }

    public void ChangeEndDate(DateTime endDate)
    {
        EndDate = endDate;
    }

    public void ChangeDescription(string description)
    {
        Guard.NullOrWhiteSpace(description, nameof(description));
        Description = description;
    }

    public void ChangeOrganization(Organization organization)
    {
        Organization = organization;
        OrgId = organization.Id;
    }

    public void CHangeOrganization(string orgId)
    {
        OrgId = orgId;
    }
}
