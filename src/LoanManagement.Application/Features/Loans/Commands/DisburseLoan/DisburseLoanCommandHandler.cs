using LoanManagement.Application.Common.Exceptions;
using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Features.Loans.Commands.DisburseLoan;

public sealed class DisburseLoanCommandHandler : IRequestHandler<DisburseLoanCommand, DisburseLoanResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;

    public DisburseLoanCommandHandler(IApplicationDbContext db, IDateTimeProvider clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<DisburseLoanResponse> Handle(DisburseLoanCommand request, CancellationToken cancellationToken)
    {
        var loan = await _db.Loans.FirstOrDefaultAsync(x => x.Id == request.LoanId && x.OrganizationId == request.OrganizationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Loan), request.LoanId);

        var now = _clock.UtcNow;
        loan.Disburse(request.DisbursedAmount, now, request.FirstRepaymentDueDate);

        var disbursementRef = $"DISB_{now:yyyyMMddHHmmss}_{Random.Shared.Next(100,999)}";
        var journal = new GlJournalEntry($"GL_{Guid.NewGuid():N}"[..16], request.OrganizationId, DateOnly.FromDateTime(now.UtcDateTime), $"Loan disbursement {loan.Id}");
        journal.AddLine("1200-LoansReceivable", request.DisbursedAmount, 0m);
        journal.AddLine("1100-Cash", 0m, request.DisbursedAmount);
        journal.ValidateDoubleEntry();
        journal.MarkCreated(now);

        _db.GlJournalEntries.Add(journal);
        await _db.SaveChangesAsync(cancellationToken);

        return new DisburseLoanResponse(loan.Id, loan.Status.ToString(), disbursementRef, now, request.FirstRepaymentDueDate);
    }
}
