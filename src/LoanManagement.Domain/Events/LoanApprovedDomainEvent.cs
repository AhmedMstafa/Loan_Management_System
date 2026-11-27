using LoanManagement.Domain.Common;

namespace LoanManagement.Domain.Events;

public sealed record LoanApprovedDomainEvent(
    string LoanId,
    string OrganizationId,
    decimal ApprovedAmount,
    string ApprovedBy,
    DateTimeOffset OccurredOn) : IDomainEvent;
