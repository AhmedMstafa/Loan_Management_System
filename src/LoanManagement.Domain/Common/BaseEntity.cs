namespace LoanManagement.Domain.Common;

public abstract class BaseEntity<TId> : IHasDomainEvents
{
    public TId Id { get; protected set; } = default!;
    public DateTimeOffset CreatedAt { get; protected set; }
    public string? CreatedBy { get; protected set; }
    public DateTimeOffset? UpdatedAt { get; protected set; }
    public bool IsDeleted { get; protected set; }

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void RemoveDomainEvent(IDomainEvent domainEvent) => _domainEvents.Remove(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();

    public void MarkCreated(DateTimeOffset utcNow, string? actor = null)
    {
        CreatedAt = utcNow;
        CreatedBy = actor;
    }

    public void MarkUpdated(DateTimeOffset utcNow)
    {
        UpdatedAt = utcNow;
    }

    public void SoftDelete(DateTimeOffset utcNow)
    {
        IsDeleted = true;
        UpdatedAt = utcNow;
    }
}

public interface IHasDomainEvents
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}
