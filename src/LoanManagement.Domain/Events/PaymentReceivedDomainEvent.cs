using LoanManagement.Domain.Common;

namespace LoanManagement.Domain.Events;

public sealed record PaymentReceivedDomainEvent(
    string LoanId,
    string PaymentId,
    string OrganizationId,
    decimal AmountPaid,
    DateTimeOffset OccurredOn) : IDomainEvent;
