using Decryptcode.Assessment.Service.Domain.Guards;
using System.Diagnostics.CodeAnalysis;

namespace Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;

public sealed class TimeEntry : BaseEntity
{
    public string UserId { get; private set; }

    public string ProjectId { get; private set; }

    public DateTime Date { get; private set; }

    public int Hours { get; private set; }

    public string Description { get; private set; }

    public User? User { get; private set; }

    public Project? Project { get; private set; }

    private TimeEntry()
    {
        // For EF Core
    }

    [SetsRequiredMembers]
    private TimeEntry(
        string id,
        string userId,
        string projectId,
        DateTime date,
        int hours,
        string description)
    {
        Guard.NullOrWhiteSpace(id);
        Guard.NullOrWhiteSpace(userId);
        Guard.NullOrWhiteSpace(projectId);
        Guard.OutOfRange(hours, 0, 24);
        Guard.NullOrWhiteSpace(description);

        Id = id;
        UserId = userId;
        ProjectId = projectId;
        Date = date;
        Hours = hours;
        Description = description;
    }

    public static TimeEntry Create(
        string id,
        string userId,
        string projectId,
        DateTime date,
        int hours,
        string description)
    {
        return new TimeEntry(id, userId, projectId, date, hours, description);
    }

    public void ChangeDate(DateTime date)
    {
        Date = date;
    }

    public void ChangeDescription(string description)
    {
        Guard.NullOrWhiteSpace(description);
        Description = description;
    }

    public void ChangeHours(int hours)
    {
        Guard.OutOfRange(hours, 0, 24);
        Hours = hours;
    }

    public void ChangeProject(string projectId)
    {
        Guard.NullOrWhiteSpace(projectId);
        ProjectId = projectId;
    }

    public void ChangeUser(string userId)
    {
        Guard.NullOrWhiteSpace(userId);
        UserId = userId;
    }

    public void ChangeProject(Project project)
    {
        Guard.Null(project);
        Project = project;
        ProjectId = project.Id;
    }

    public void ChangeUser(User user)
    {
        Guard.Null(user);
        User = user;
        UserId = user.Id;
    }
}
