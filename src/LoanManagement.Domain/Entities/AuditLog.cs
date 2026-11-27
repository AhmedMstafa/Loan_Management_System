using LoanManagement.Domain.Common;

namespace LoanManagement.Domain.Entities;

public sealed class AuditLog : BaseEntity<long>
{
    private AuditLog() { }

    public AuditLog(
        string organizationId,
        string actorId,
        string actorRole,
        string action,
        string entity,
        string entityId,
        string? ipAddress,
        string? oldValues,
        string? newValues,
        DateTimeOffset timestamp)
    {
        OrganizationId = organizationId;
        ActorId = actorId;
        ActorRole = actorRole;
        Action = action;
        Entity = entity;
        EntityId = entityId;
        IpAddress = ipAddress;
        OldValues = oldValues;
        NewValues = newValues;
        Timestamp = timestamp;
        CreatedAt = timestamp;
    }

    public DateTimeOffset Timestamp { get; private set; }
    public string OrganizationId { get; private set; } = string.Empty;
    public string ActorId { get; private set; } = string.Empty;
    public string ActorRole { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty;
    public string Entity { get; private set; } = string.Empty;
    public string EntityId { get; private set; } = string.Empty;
    public string? IpAddress { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
}
