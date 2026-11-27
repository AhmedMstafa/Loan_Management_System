using LoanManagement.Domain.Common;

namespace LoanManagement.Domain.Events;

public sealed record LoanDisbursedDomainEvent(
    string LoanId,
    string OrganizationId,
    decimal DisbursedAmount,
    DateTimeOffset OccurredOn) : IDomainEvent;
