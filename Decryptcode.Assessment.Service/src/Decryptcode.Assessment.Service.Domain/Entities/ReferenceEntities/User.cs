using Decryptcode.Assessment.Service.Domain.Entities.AggregateRoots;
using Decryptcode.Assessment.Service.Domain.Guards;
using System.Diagnostics.CodeAnalysis;

namespace Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;

public sealed class User : BaseEntity
{
    public string OrgId { get; private set; }

    public string Email { get; private set; }

    public string Name { get; private set; }

    // Altought this be a string, the best approach would be to have a dedicated entity for this,
    // with a predefined list of Roles, but also allowing custom ones,
    // but for the sake of simplicity, we will keep it as a string.
    // Or dedicated endpoint that returns the list of supported Roles, and validate against it.
    public string Role { get; private set; }

    public bool Active { get; private set; }

    public string Bio { get; private set; }

    public Organization? Organization { get; private set; }

    public ICollection<TimeEntry> TimeEntries { get; private set; } = new HashSet<TimeEntry>();

    private User()
    {
        // For EF
    }

    [SetsRequiredMembers]
    private User(string id, string orgId, string email, string name, string role, bool active, string bio)
    {
        Guard.NullOrEmpty(email);
        Guard.NullOrEmpty(name);
        Guard.NullOrEmpty(role);
        Guard.NullOrEmpty(bio);

        Id = id;
        OrgId = orgId;
        Email = email;
        Name = name;
        Role = role;
        Active = active;
        Bio = bio;
    }

    public static User Create(string id, string orgId, string email, string name, string roles, bool active, string bio)
    {
        return new User(id, orgId, email, name, roles, active, bio);
    }

    public void ChangeEmail(string email)
    {
        Guard.NullOrEmpty(email);
        Email = email;
    }

    public void ChangeName(string name)
    {
        Guard.NullOrEmpty(name);
        Name = name;
    }

    public void ChangeRole(string role)
    {
        Guard.NullOrEmpty(role);
        Role = role;
    }

    public void Activate()
    {
        Active = true;
    }

    public void Deactivate()
    {
        Active = false;
    }

    public void ChangeBio(string bio)
    {
        Guard.NullOrEmpty(bio);
        Bio = bio;
    }
}
