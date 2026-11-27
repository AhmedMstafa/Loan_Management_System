using LoanManagement.Application.Common.Exceptions;
using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Domain.Entities;
using LoanManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Features.Repayments.Commands.MakeRepayment;

public sealed class MakeRepaymentCommandHandler : IRequestHandler<MakeRepaymentCommand, MakeRepaymentResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;

    public MakeRepaymentCommandHandler(IApplicationDbContext db, IDateTimeProvider clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<MakeRepaymentResponse> Handle(MakeRepaymentCommand request, CancellationToken cancellationToken)
    {
        var loan = await _db.Loans
            .Include(x => x.RepaymentSchedule)
            .FirstOrDefaultAsync(x => x.Id == request.LoanId && x.OrganizationId == request.OrganizationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Loan), request.LoanId);

        var amount = decimal.Round(request.PaymentAmount, 4, MidpointRounding.ToEven);
        if (amount <= 0) throw new FluentValidation.ValidationException("Payment amount must be greater than zero.");

        // Allocate: interest first (accrued interest), then principal, capped to outstanding principal.
        var interestPortion = Math.Min(amount, decimal.Round(loan.AccruedInterest, 4, MidpointRounding.ToEven));
        var remainingAfterInterest = decimal.Round(amount - interestPortion, 4, MidpointRounding.ToEven);
        var principalPortion = Math.Min(remainingAfterInterest, decimal.Round(loan.OutstandingPrincipal, 4, MidpointRounding.ToEven));
        var allocated = decimal.Round(interestPortion + principalPortion, 4, MidpointRounding.ToEven);

        // If there's any unallocated remainder (overpayment) keep it in principal up to zero; otherwise treat as principal reduction not possible.
        if (allocated != amount)
        {
            // adjust principal to match full amount when within bounds, else clamp
            principalPortion = decimal.Round(amount - interestPortion, 4, MidpointRounding.ToEven);
            if (principalPortion > loan.OutstandingPrincipal)
                principalPortion = loan.OutstandingPrincipal;
            allocated = decimal.Round(interestPortion + principalPortion, 4, MidpointRounding.ToEven);
            if (allocated != amount)
                throw new FluentValidation.ValidationException("Payment amount exceeds allocatable loan balance.");
        }

        var now = _clock.UtcNow;
        var paymentId = $"pmt_{Guid.NewGuid():N}"[..16];
        var payment = loan.RecordRepayment(
            paymentId,
            amount,
            principalPortion,
            interestPortion,
            0m,
            request.PaymentMethod,
            request.PaymentReference,
            request.PaidAt == default ? now : request.PaidAt);

        ApplyToSchedule(loan, principalPortion, interestPortion, request.PaidAt == default ? now : request.PaidAt);

        // GL: Debit Cash, Credit Loans Receivable (principal), Credit Interest Income (interest)
        var journal = new GlJournalEntry($"GL_{Guid.NewGuid():N}"[..16], request.OrganizationId, DateOnly.FromDateTime((request.PaidAt == default ? now : request.PaidAt).UtcDateTime), $"Loan repayment {loan.Id} / {paymentId}");
        journal.AddLine("1100-Cash", amount, 0m);
        if (principalPortion > 0m) journal.AddLine("1200-LoansReceivable", 0m, principalPortion);
        if (interestPortion > 0m) journal.AddLine("4100-InterestIncome", 0m, interestPortion);
        if (journal.Lines.Count == 1) journal.AddLine("4200-FeeIncome", 0m, 0m); // shouldn't happen; guard
        journal.ValidateDoubleEntry();
        journal.MarkCreated(now);
        _db.GlJournalEntries.Add(journal);

        await _db.SaveChangesAsync(cancellationToken);

        return new MakeRepaymentResponse(payment.Id, loan.Id, payment.AmountPaid, payment.PrincipalAllocated, payment.InterestAllocated, payment.LateFeeAllocated, loan.OutstandingPrincipal, payment.ProcessedAt);
    }

    private static void ApplyToSchedule(Loan loan, decimal principalPortion, decimal interestPortion, DateTimeOffset paidAt)
    {
        var remainingInterest = interestPortion;
        var remainingPrincipal = principalPortion;
        foreach (var item in loan.RepaymentSchedule.OrderBy(x => x.InstallmentNumber))
        {
            if (item.Status == RepaymentStatus.Paid) continue;
            var interestApplied = Math.Min(remainingInterest, item.InterestDue);
            var principalApplied = Math.Min(remainingPrincipal, item.PrincipalDue);
            remainingInterest = decimal.Round(remainingInterest - interestApplied, 4, MidpointRounding.ToEven);
            remainingPrincipal = decimal.Round(remainingPrincipal - principalApplied, 4, MidpointRounding.ToEven);

            if (interestApplied >= item.InterestDue && principalApplied >= item.PrincipalDue)
                item.MarkPaid(paidAt);
            else if (interestApplied > 0 || principalApplied > 0)
                item.MarkPartiallyPaid();

            if (remainingInterest <= 0 && remainingPrincipal <= 0) break;
        }
    }
}
