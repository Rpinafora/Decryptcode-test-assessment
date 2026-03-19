using Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;
using Decryptcode.Assessment.Service.Domain.Entities.ValueObjects;
using Decryptcode.Assessment.Service.Domain.Guards;
using System.Diagnostics.CodeAnalysis;

namespace Decryptcode.Assessment.Service.Domain.Entities.AggregateRoots;

public sealed class Organization : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; }

    public string Slug { get; private set; }

    // Altought this be a string, the best approach would be to have a dedicated entity for this,
    // with a predefined list of industries, but also allowing custom ones,
    // but for the sake of simplicity, we will keep it as a string.
    // Or dedicated endpoint that returns the list of supported Industries, and validate against it.
    public string Industry { get; private set; }

    // Altought this be a string, the best approach would be to have a dedicated entity for this,
    // with a predefined list of tiers, but also allowing custom ones,
    // but for the sake of simplicity, we will keep it as a string.
    // Or dedicated endpoint that returns the list of supported Tiers, and validate against it.
    public string Tier { get; private set; }

    public string ContactEmail { get; private set; }

    public string Description { get; private set; }

    public Settings Settings { get; private set; }

    public Metadata Metadata { get; private set; }

    public ICollection<User> Users { get; private set; } = new HashSet<User>();

    public ICollection<Project> Projects { get; private set; } = new HashSet<Project>();

    public ICollection<Invoice> Invoices { get; private set; } = new HashSet<Invoice>();

    // Parameterless constructor is required by EF Core, but we can make it private to prevent its usage outside of the ORM.
    private Organization()
    {
    }

    [SetsRequiredMembers]
    private Organization(
        string id,
        string name,
        string slug,
        string industry,
        string tier,
        string contactEmail,
        string description,
        Settings settings,
        Metadata metadata)
    {
        Guard.NullOrEmpty(id);
        Guard.NullOrEmpty(name);
        Guard.NullOrEmpty(slug);
        Guard.NullOrEmpty(industry);
        Guard.NullOrEmpty(tier);
        Guard.NullOrEmpty(contactEmail);
        Guard.NullOrEmpty(description);
        Guard.Null(settings);
        Guard.Null(metadata);

        Id = id;
        Name = name;
        Slug = slug;
        Industry = industry;
        Tier = tier;
        ContactEmail = contactEmail;
        Description = description;
        Settings = settings;
        Metadata = metadata;
    }

    public static Organization Create(
        string id,
        string name,
        string slug,
        string industry,
        string tier,
        string contactEmail,
        string description,
        Settings settings,
        Metadata metadata)
    {
        return new Organization(id, name, slug, industry, tier, contactEmail, description, settings, metadata);
    }

    public void ChangeName(string name)
    {
        Guard.NullOrWhiteSpace(name, nameof(name));
        Name = name;
    }

    public void ChangeSlug(string slug)
    {
        Guard.NullOrWhiteSpace(slug, nameof(slug));
        Slug = slug;
    }

    public void ChangeIndustry(string industry)
    {
        Guard.NullOrEmpty(industry);
        Industry = industry;
    }

    public void ChangeTier(string tier)
    {
        Guard.NullOrEmpty(tier);
        Tier = tier;
    }

    public void ChangeContactEmail(string contactEmail)
    {
        Guard.NullOrWhiteSpace(contactEmail, nameof(contactEmail));
        ContactEmail = contactEmail;
    }

    public void ChangeDescription(string description)
    {
        Guard.NullOrEmpty(description);
        Description = description;
    }

    public void ChangeSettings(Settings settings)
    {
        Guard.Null(settings);
        Settings = settings;
    }

    public void ChangeMetadata(Metadata metadata)
    {
        Guard.Null(metadata);
        Metadata = metadata;
    }
}
