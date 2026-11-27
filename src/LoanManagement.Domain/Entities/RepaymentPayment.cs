using LoanManagement.Domain.Common;
using LoanManagement.Domain.Enums;

namespace LoanManagement.Domain.Entities;

public sealed class RepaymentPayment : BaseEntity<string>
{
    private RepaymentPayment() { }

    public RepaymentPayment(
        string id,
        string loanId,
        string organizationId,
        decimal amountPaid,
        decimal principalAllocated,
        decimal interestAllocated,
        decimal lateFeeAllocated,
        PaymentMethod paymentMethod,
        string transactionReference,
        DateTimeOffset processedAt)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Payment id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(loanId)) throw new ArgumentException("Loan id is required.", nameof(loanId));
        if (amountPaid <= 0) throw new ArgumentOutOfRangeException(nameof(amountPaid));

        Id = id;
        LoanId = loanId;
        OrganizationId = organizationId;
        AmountPaid = decimal.Round(amountPaid, 4, MidpointRounding.ToEven);
        PrincipalAllocated = decimal.Round(principalAllocated, 4, MidpointRounding.ToEven);
        InterestAllocated = decimal.Round(interestAllocated, 4, MidpointRounding.ToEven);
        LateFeeAllocated = decimal.Round(lateFeeAllocated, 4, MidpointRounding.ToEven);
        PaymentMethod = paymentMethod;
        TransactionReference = transactionReference;
        ProcessedAt = processedAt;
    }

    public string LoanId { get; private set; } = string.Empty;
    public string OrganizationId { get; private set; } = string.Empty;
    public decimal AmountPaid { get; private set; }
    public decimal PrincipalAllocated { get; private set; }
    public decimal InterestAllocated { get; private set; }
    public decimal LateFeeAllocated { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public string TransactionReference { get; private set; } = string.Empty;
    public DateTimeOffset ProcessedAt { get; private set; }
}
