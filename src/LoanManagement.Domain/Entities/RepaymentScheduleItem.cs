using LoanManagement.Domain.Common;
using LoanManagement.Domain.Enums;

namespace LoanManagement.Domain.Entities;

public sealed class RepaymentScheduleItem : BaseEntity<long>
{
    private RepaymentScheduleItem() { }

    public RepaymentScheduleItem(
        string loanId,
        int installmentNumber,
        DateOnly dueDate,
        decimal principalDue,
        decimal interestDue,
        decimal remainingPrincipal)
    {
        if (installmentNumber <= 0) throw new ArgumentOutOfRangeException(nameof(installmentNumber));
        if (principalDue < 0 || interestDue < 0) throw new ArgumentException("Schedule amounts cannot be negative.");

        LoanId = loanId;
        InstallmentNumber = installmentNumber;
        DueDate = dueDate;
        PrincipalDue = decimal.Round(principalDue, 4, MidpointRounding.ToEven);
        InterestDue = decimal.Round(interestDue, 4, MidpointRounding.ToEven);
        TotalDue = decimal.Round(PrincipalDue + InterestDue, 4, MidpointRounding.ToEven);
        RemainingPrincipal = decimal.Round(remainingPrincipal, 4, MidpointRounding.ToEven);
        Status = RepaymentStatus.Pending;
    }

    public string LoanId { get; private set; } = string.Empty;
    public int InstallmentNumber { get; private set; }
    public DateOnly DueDate { get; private set; }
    public decimal PrincipalDue { get; private set; }
    public decimal InterestDue { get; private set; }
    public decimal TotalDue { get; private set; }
    public decimal RemainingPrincipal { get; private set; }
    public RepaymentStatus Status { get; private set; }
    public DateTimeOffset? PaidDate { get; private set; }

    public void MarkPaid(DateTimeOffset paidAt)
    {
        Status = RepaymentStatus.Paid;
        PaidDate = paidAt;
    }

    public void MarkPartiallyPaid()
    {
        Status = RepaymentStatus.PartiallyPaid;
    }

    public void MarkOverdue()
    {
        if (Status == RepaymentStatus.Pending)
            Status = RepaymentStatus.Overdue;
    }
}
