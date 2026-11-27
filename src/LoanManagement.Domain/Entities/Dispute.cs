using LoanManagement.Domain.Common;
using LoanManagement.Domain.Enums;

namespace LoanManagement.Domain.Entities;

public sealed class Dispute : BaseEntity<string>, IAggregateRoot
{
    private Dispute() { }

    public Dispute(string id, string organizationId, string loanId, string reason, DateTimeOffset openedAt)
    {
        Id = id;
        OrganizationId = organizationId;
        LoanId = loanId;
        Reason = reason?.Trim() ?? string.Empty;
        Status = DisputeStatus.Open;
        OpenedAt = openedAt;
    }

    public string OrganizationId { get; private set; } = string.Empty;
    public string LoanId { get; private set; } = string.Empty;
    public string Reason { get; private set; } = string.Empty;
    public DisputeStatus Status { get; private set; }
    public DateTimeOffset OpenedAt { get; private set; }
    public DateTimeOffset? ResolvedAt { get; private set; }
    public string? ResolvedBy { get; private set; }
    public string? ResolutionType { get; private set; }
    public decimal? AdjustmentAmount { get; private set; }
    public string? ResolutionNotes { get; private set; }

    public void Resolve(string resolvedBy, string resolutionType, decimal? adjustmentAmount, string? resolutionNotes, DateTimeOffset resolvedAt)
    {
        Status = DisputeStatus.Resolved;
        ResolvedBy = resolvedBy;
        ResolutionType = resolutionType;
        AdjustmentAmount = adjustmentAmount.HasValue
            ? decimal.Round(adjustmentAmount.Value, 4, MidpointRounding.ToEven)
            : null;
        ResolutionNotes = resolutionNotes;
        ResolvedAt = resolvedAt;
    }
}
