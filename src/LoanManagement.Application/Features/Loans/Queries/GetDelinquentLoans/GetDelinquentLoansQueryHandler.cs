using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Application.Common.Models;
using LoanManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Features.Loans.Queries.GetDelinquentLoans;

public sealed class GetDelinquentLoansQueryHandler : IRequestHandler<GetDelinquentLoansQuery, PaginatedList<DelinquentLoanDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;
    public GetDelinquentLoansQueryHandler(IApplicationDbContext db, IDateTimeProvider clock) { _db = db; _clock = clock; }

    public async Task<PaginatedList<DelinquentLoanDto>> Handle(GetDelinquentLoansQuery request, CancellationToken cancellationToken)
    {
        var today = _clock.UtcToday;
        var baseQuery = _db.Loans.AsNoTracking()
            .Where(x => x.OrganizationId == request.OrganizationId && (x.Status == LoanStatus.Delinquent || x.Status == LoanStatus.Active || x.Status == LoanStatus.Disbursed))
            .Select(x => new
            {
                Loan = x,
                Borrower = _db.Borrowers.Where(b => b.Id == x.BorrowerId).Select(b => b.FirstName + " " + b.LastName).FirstOrDefault(),
                OverdueItems = _db.RepaymentScheduleItems.Where(s => s.LoanId == x.Id && (s.Status == RepaymentStatus.Overdue || (s.Status == RepaymentStatus.Pending && s.DueDate < today)))
            });

        var list = await baseQuery.ToListAsync(cancellationToken);
        var mapped = list.Select(x =>
        {
            var overdue = x.OverdueItems.ToList();
            var daysOverdue = overdue.Count == 0 ? 0 : (today.DayNumber - overdue.Min(i => i.DueDate).DayNumber);
            var lastPayment = _db.RepaymentPayments.Where(p => p.LoanId == x.Loan.Id).OrderByDescending(p => p.ProcessedAt).Select(p => DateOnly.FromDateTime(p.ProcessedAt.UtcDateTime)).FirstOrDefault();
            return new DelinquentLoanDto(x.Loan.Id, x.Borrower ?? "", daysOverdue, overdue.Sum(i => i.TotalDue), x.Loan.OutstandingPrincipal, daysOverdue > 90 ? "Substandard" : daysOverdue > 30 ? "Watch" : "Current", lastPayment == default ? null : lastPayment);
        })
        .Where(x => x.DaysOverdue >= request.MinDaysOverdue)
        .OrderByDescending(x => x.DaysOverdue)
        .ToList();

        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);
        var paged = mapped.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedList<DelinquentLoanDto>(paged, mapped.Count, pageNumber, pageSize);
    }
}
