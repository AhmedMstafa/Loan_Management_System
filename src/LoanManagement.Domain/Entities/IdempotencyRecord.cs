using LoanManagement.Domain.Common;

namespace LoanManagement.Domain.Entities;

public sealed class IdempotencyRecord : BaseEntity<string>
{
    private IdempotencyRecord() { }

    public IdempotencyRecord(
        string idempotencyKey,
        string organizationId,
        string requestHash,
        int statusCode,
        string responsePayload,
        DateTimeOffset createdAt,
        DateTimeOffset expiresAt)
    {
        Id = idempotencyKey;
        OrganizationId = organizationId;
        RequestHash = requestHash;
        StatusCode = statusCode;
        ResponsePayload = responsePayload;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public string OrganizationId { get; private set; } = string.Empty;
    public string RequestHash { get; private set; } = string.Empty;
    public int StatusCode { get; private set; }
    public string ResponsePayload { get; private set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; private set; }
}
