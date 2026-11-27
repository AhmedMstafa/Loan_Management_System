using LoanManagement.Application.Common.Exceptions;
using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Features.Loans.Commands.RunCreditCheck;

public sealed class RunCreditCheckCommandHandler : IRequestHandler<RunCreditCheckCommand, RunCreditCheckResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly ICreditBureauService _creditBureau;
    private readonly IDateTimeProvider _clock;

    public RunCreditCheckCommandHandler(IApplicationDbContext db, ICreditBureauService creditBureau, IDateTimeProvider clock)
    {
        _db = db;
        _creditBureau = creditBureau;
        _clock = clock;
    }

    public async Task<RunCreditCheckResponse> Handle(RunCreditCheckCommand request, CancellationToken cancellationToken)
    {
        var loan = await _db.Loans.FirstOrDefaultAsync(x => x.Id == request.LoanId && x.OrganizationId == request.OrganizationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Loan), request.LoanId);

        var result = await _creditBureau.CheckAsync(loan.BorrowerId, request.Provider, request.ForceRefresh, cancellationToken);
        var check = new CreditCheck(
            id: $"cc_{Guid.NewGuid():N}"[..16],
            organizationId: request.OrganizationId,
            loanId: loan.Id,
            provider: request.Provider,
            creditScore: result.CreditScore,
            riskRating: result.RiskRating,
            debtToIncomeRatio: result.DebtToIncomeRatio,
            recommendation: result.Recommendation,
            checkedAt: result.CheckedAt);
        check.MarkCreated(_clock.UtcNow);

        loan.MarkUnderwriting();
        _db.CreditChecks.Add(check);
        await _db.SaveChangesAsync(cancellationToken);

        return new RunCreditCheckResponse(loan.Id, result.CreditScore, result.RiskRating, result.DebtToIncomeRatio, result.Recommendation, result.CheckedAt);
    }
}
