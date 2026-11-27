using LoanManagement.Application.Common.Exceptions;
using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Features.Loans.Queries.GetLoanStatement;

public sealed class GetLoanStatementQueryHandler : IRequestHandler<GetLoanStatementQuery, GetLoanStatementResponse>
{
    private readonly IApplicationDbContext _db;
    public GetLoanStatementQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<GetLoanStatementResponse> Handle(GetLoanStatementQuery request, CancellationToken cancellationToken)
    {
        var loan = await _db.Loans.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.LoanId && x.OrganizationId == request.OrganizationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Loan), request.LoanId);

        var payments = await _db.RepaymentPayments.AsNoTracking()
            .Where(x => x.LoanId == loan.Id && DateOnly.FromDateTime(x.ProcessedAt.UtcDateTime) >= request.From && DateOnly.FromDateTime(x.ProcessedAt.UtcDateTime) <= request.To)
            .ToListAsync(cancellationToken);

        var totalInterest = payments.Sum(x => x.InterestAllocated);
        var totalPrincipal = payments.Sum(x => x.PrincipalAllocated);
        var closing = loan.OutstandingPrincipal;
        var opening = decimal.Round(closing + totalPrincipal, 4, MidpointRounding.ToEven);

        var url = $"https://storage.bank.internal/statements/{loan.Id}_{request.To:yyyyMMdd}.{(request.Format?.ToLowerInvariant() ?? "pdf")}";
        return new GetLoanStatementResponse(loan.Id, url, opening, closing, totalInterest, totalPrincipal);
    }
}
