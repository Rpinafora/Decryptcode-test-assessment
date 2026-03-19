namespace Decryptcode.Assessment.Service.Domain.Entities;

public abstract class BaseEntity
{
    public required string Id { get; set; }

    public DateTime CreatedAt { get; protected set; }

    public string? CreatedById { get; protected set; }

    public DateTime? UpdatedAt { get; protected set; }

    public string? UpdatedById { get; protected set; }

    public DateTime? DeletedAt { get; protected set; }

    public string? DeletedBy { get; protected set; }

    private readonly List<object> _domainEvents = [];

    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(object @event)
    {
        _domainEvents.Add(@event);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void ChangeCreatedInfo(string? userId)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedById = userId;
    }

    public void ChangeUpdatedInfo(string? userId)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedById = userId;
    }

    public void ChangeDeletedInfo(string? userId)
    {
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }
}
