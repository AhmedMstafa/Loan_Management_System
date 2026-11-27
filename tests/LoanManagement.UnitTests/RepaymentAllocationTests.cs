using FluentAssertions;
using LoanManagement.Domain.Entities;
using LoanManagement.Domain.Enums;

namespace LoanManagement.UnitTests;

public class RepaymentAllocationTests
{
    [Fact]
    public void RecordRepayment_ShouldAllocatePrincipalInterestAndLateFee_AndUpdateBalances()
    {
        var loan = CreateActiveLoan();
        loan.Disburse(1000m, DateTimeOffset.UtcNow, new DateOnly(2026, 2, 1));
        loan.AccrueInterest(50m);

        var payment = loan.RecordRepayment(
            "pmt_1",
            amountPaid: 300m,
            principalAllocated: 230m,
            interestAllocated: 50m,
            lateFeeAllocated: 20m,
            paymentMethod: PaymentMethod.BankTransfer,
            transactionReference: "REF-1",
            processedAt: DateTimeOffset.UtcNow);

        payment.PrincipalAllocated.Should().Be(230m);
        payment.InterestAllocated.Should().Be(50m);
        payment.LateFeeAllocated.Should().Be(20m);
        loan.OutstandingPrincipal.Should().Be(770m);
        loan.AccruedInterest.Should().Be(0m);
        loan.Status.Should().Be(LoanStatus.Active);
    }

    [Fact]
    public void RecordRepayment_ShouldCloseLoan_WhenFullyRepaid()
    {
        var loan = CreateActiveLoan();
        loan.Disburse(500m, DateTimeOffset.UtcNow, new DateOnly(2026, 2, 1));

        loan.RecordRepayment("pmt_2", 500m, 500m, 0m, 0m, PaymentMethod.Cash, "REF-2", DateTimeOffset.UtcNow);

        loan.OutstandingPrincipal.Should().Be(0m);
        loan.Status.Should().Be(LoanStatus.Closed);
    }

    private static Loan CreateActiveLoan()
    {
        var loan = new Loan("loa_1", "org1", "bor_1", "prd_1", 1000m, "USD", 12, 12m, "Test", null);
        loan.MarkCreated(DateTimeOffset.UtcNow);
        loan.Approve(1000m, 12m, 12, "uw", DateTimeOffset.UtcNow);
        return loan;
    }
}
