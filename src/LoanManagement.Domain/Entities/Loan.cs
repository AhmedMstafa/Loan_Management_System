using LoanManagement.Domain.Common;
using LoanManagement.Domain.Enums;
using LoanManagement.Domain.Events;
using LoanManagement.Domain.Exceptions;

namespace LoanManagement.Domain.Entities;

public sealed class Loan : BaseEntity<string>, IAggregateRoot
{
    private readonly List<RepaymentScheduleItem> _repaymentSchedule = [];
    private readonly List<RepaymentPayment> _payments = [];

    private Loan() { }

    public Loan(
        string id,
        string organizationId,
        string borrowerId,
        string productId,
        decimal requestedAmount,
        string currency,
        int tenorMonths,
        decimal annualInterestRate,
        string? purpose = null,
        string? collateralDescription = null)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new DomainValidationException(nameof(id), "Loan id is required.");
        if (string.IsNullOrWhiteSpace(organizationId)) throw new DomainValidationException(nameof(organizationId), "Organization id is required.");
        if (string.IsNullOrWhiteSpace(borrowerId)) throw new DomainValidationException(nameof(borrowerId), "Borrower id is required.");
        if (string.IsNullOrWhiteSpace(productId)) throw new DomainValidationException(nameof(productId), "Product id is required.");
        if (requestedAmount <= 0) throw new DomainValidationException(nameof(requestedAmount), "Requested amount must be greater than zero.");
        if (tenorMonths <= 0) throw new DomainValidationException(nameof(tenorMonths), "Tenor must be greater than zero.");
        if (annualInterestRate < 0) throw new DomainValidationException(nameof(annualInterestRate), "Interest rate cannot be negative.");

        Id = id;
        OrganizationId = organizationId;
        BorrowerId = borrowerId;
        ProductId = productId;
        RequestedAmount = decimal.Round(requestedAmount, 4, MidpointRounding.ToEven);
        Currency = string.IsNullOrWhiteSpace(currency) ? "USD" : currency.Trim().ToUpperInvariant();
        TenorMonths = tenorMonths;
        InterestRate = decimal.Round(annualInterestRate, 4, MidpointRounding.ToEven);
        Purpose = purpose?.Trim();
        CollateralDescription = collateralDescription?.Trim();
        Status = LoanStatus.Draft;
        OutstandingPrincipal = 0m;
        AccruedInterest = 0m;
    }

    public string OrganizationId { get; private set; } = string.Empty;
    public string BorrowerId { get; private set; } = string.Empty;
    public string ProductId { get; private set; } = string.Empty;
    public LoanStatus Status { get; private set; }
    public decimal RequestedAmount { get; private set; }
    public decimal? ApprovedAmount { get; private set; }
    public decimal? DisbursedAmount { get; private set; }
    public string Currency { get; private set; } = "USD";
    public decimal InterestRate { get; private set; }
    public int TenorMonths { get; private set; }
    public decimal OutstandingPrincipal { get; private set; }
    public decimal AccruedInterest { get; private set; }
    public DateTimeOffset? DisbursedAt { get; private set; }
    public DateOnly? FirstRepaymentDate { get; private set; }
    public string? ApprovedBy { get; private set; }
    public string? Purpose { get; private set; }
    public string? CollateralDescription { get; private set; }
    public int ConcurrencyToken { get; private set; } = 1;

    public IReadOnlyCollection<RepaymentScheduleItem> RepaymentSchedule => _repaymentSchedule.AsReadOnly();
    public IReadOnlyCollection<RepaymentPayment> Payments => _payments.AsReadOnly();

    public void Submit()
    {
        EnsureStatus(LoanStatus.Draft, nameof(Submit));
        Status = LoanStatus.Submitted;
    }

    public void MarkUnderwriting()
    {
        if (Status is not (LoanStatus.Submitted or LoanStatus.Draft))
            throw new InvalidLoanStateException(Id, Status.ToString(), nameof(MarkUnderwriting));
        Status = LoanStatus.Underwriting;
    }

    public void Approve(decimal approvedAmount, decimal approvedInterestRate, int tenorMonths, string approvedBy, DateTimeOffset approvedAt)
    {
        if (Status is not (LoanStatus.Submitted or LoanStatus.Underwriting or LoanStatus.Draft))
            throw new InvalidLoanStateException(Id, Status.ToString(), nameof(Approve));
        if (approvedAmount <= 0) throw new DomainValidationException(nameof(approvedAmount), "Approved amount must be greater than zero.");
        if (tenorMonths <= 0) throw new DomainValidationException(nameof(tenorMonths), "Tenor must be greater than zero.");
        if (string.IsNullOrWhiteSpace(approvedBy)) throw new DomainValidationException(nameof(approvedBy), "Approver is required.");

        ApprovedAmount = decimal.Round(approvedAmount, 4, MidpointRounding.ToEven);
        InterestRate = decimal.Round(approvedInterestRate, 4, MidpointRounding.ToEven);
        TenorMonths = tenorMonths;
        ApprovedBy = approvedBy;
        Status = LoanStatus.Approved;

        AddDomainEvent(new LoanApprovedDomainEvent(Id, OrganizationId, ApprovedAmount.Value, approvedBy, approvedAt));
    }

    public void Disburse(decimal disbursedAmount, DateTimeOffset disbursedAt, DateOnly firstRepaymentDueDate)
    {
        EnsureStatus(LoanStatus.Approved, nameof(Disburse));
        if (disbursedAmount <= 0) throw new DomainValidationException(nameof(disbursedAmount), "Disbursed amount must be greater than zero.");

        DisbursedAmount = decimal.Round(disbursedAmount, 4, MidpointRounding.ToEven);
        OutstandingPrincipal = DisbursedAmount.Value;
        DisbursedAt = disbursedAt;
        FirstRepaymentDate = firstRepaymentDueDate;
        Status = LoanStatus.Disbursed;

        AddDomainEvent(new LoanDisbursedDomainEvent(Id, OrganizationId, DisbursedAmount.Value, disbursedAt));
    }

    public void Activate()
    {
        if (Status == LoanStatus.Disbursed)
            Status = LoanStatus.Active;
    }

    public void ReplaceRepaymentSchedule(IEnumerable<RepaymentScheduleItem> items)
    {
        _repaymentSchedule.Clear();
        _repaymentSchedule.AddRange(items.OrderBy(i => i.InstallmentNumber));
    }

    public RepaymentPayment RecordRepayment(
        string paymentId,
        decimal amountPaid,
        decimal principalAllocated,
        decimal interestAllocated,
        decimal lateFeeAllocated,
        PaymentMethod paymentMethod,
        string transactionReference,
        DateTimeOffset processedAt)
    {
        if (Status is not (LoanStatus.Disbursed or LoanStatus.Active or LoanStatus.Delinquent))
            throw new InvalidLoanStateException(Id, Status.ToString(), nameof(RecordRepayment));
        if (amountPaid <= 0) throw new DomainValidationException(nameof(amountPaid), "Payment amount must be greater than zero.");
        if (principalAllocated < 0 || interestAllocated < 0 || lateFeeAllocated < 0)
            throw new DomainValidationException("Allocation", "Payment allocations cannot be negative.");

        var totalAllocated = decimal.Round(principalAllocated + interestAllocated + lateFeeAllocated, 4, MidpointRounding.ToEven);
        var roundedAmount = decimal.Round(amountPaid, 4, MidpointRounding.ToEven);
        if (totalAllocated != roundedAmount)
            throw new DomainValidationException("Allocation", "Payment allocations must equal total amount paid.");

        var payment = new RepaymentPayment(
            paymentId, Id, OrganizationId, roundedAmount,
            decimal.Round(principalAllocated, 4, MidpointRounding.ToEven),
            decimal.Round(interestAllocated, 4, MidpointRounding.ToEven),
            decimal.Round(lateFeeAllocated, 4, MidpointRounding.ToEven),
            paymentMethod, transactionReference, processedAt);

        _payments.Add(payment);

        OutstandingPrincipal = decimal.Round(Math.Max(0m, OutstandingPrincipal - payment.PrincipalAllocated), 4, MidpointRounding.ToEven);
        AccruedInterest = decimal.Round(Math.Max(0m, AccruedInterest - payment.InterestAllocated), 4, MidpointRounding.ToEven);

        if (OutstandingPrincipal == 0m && AccruedInterest == 0m)
            Status = LoanStatus.Closed;
        else if (Status == LoanStatus.Disbursed)
            Status = LoanStatus.Active;

        AddDomainEvent(new PaymentReceivedDomainEvent(Id, paymentId, OrganizationId, roundedAmount, processedAt));
        return payment;
    }

    public void MarkDelinquent()
    {
        if (Status is LoanStatus.Active or LoanStatus.Disbursed)
            Status = LoanStatus.Delinquent;
    }

    public void AccrueInterest(decimal amount)
    {
        if (amount < 0) throw new DomainValidationException(nameof(amount), "Accrual amount cannot be negative.");
        AccruedInterest = decimal.Round(AccruedInterest + amount, 4, MidpointRounding.ToEven);
    }

    public void ApplyTopUp(decimal additionalAmount, int tenorExtensionMonths)
    {
        if (Status is not (LoanStatus.Active or LoanStatus.Disbursed or LoanStatus.Approved))
            throw new InvalidLoanStateException(Id, Status.ToString(), nameof(ApplyTopUp));
        if (additionalAmount <= 0) throw new DomainValidationException(nameof(additionalAmount), "Additional amount must be greater than zero.");
        if (tenorExtensionMonths < 0) throw new DomainValidationException(nameof(tenorExtensionMonths), "Tenor extension cannot be negative.");

        OutstandingPrincipal = decimal.Round(OutstandingPrincipal + additionalAmount, 4, MidpointRounding.ToEven);
        TenorMonths += tenorExtensionMonths;
    }

    private void EnsureStatus(LoanStatus expected, string action)
    {
        if (Status != expected)
            throw new InvalidLoanStateException(Id, Status.ToString(), action);
    }
}
