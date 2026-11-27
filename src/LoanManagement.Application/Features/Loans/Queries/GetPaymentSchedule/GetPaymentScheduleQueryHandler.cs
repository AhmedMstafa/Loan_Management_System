using LoanManagement.Application.Common.Exceptions;
using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Features.Loans.Queries.GetPaymentSchedule;

public sealed class GetPaymentScheduleQueryHandler : IRequestHandler<GetPaymentScheduleQuery, GetPaymentScheduleResponse>
{
    private readonly IApplicationDbContext _db;
    public GetPaymentScheduleQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<GetPaymentScheduleResponse> Handle(GetPaymentScheduleQuery request, CancellationToken cancellationToken)
    {
        var loan = await _db.Loans.AsNoTracking()
            .Include(x => x.RepaymentSchedule)
            .FirstOrDefaultAsync(x => x.Id == request.LoanId && x.OrganizationId == request.OrganizationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Loan), request.LoanId);

        var items = loan.RepaymentSchedule.OrderBy(x => x.InstallmentNumber)
            .Select(x => new PaymentScheduleItemDto(x.InstallmentNumber, x.DueDate, x.PrincipalDue, x.InterestDue, x.TotalDue, x.RemainingPrincipal, x.Status.ToString()))
            .ToList();

        return new GetPaymentScheduleResponse(loan.Id, items.Sum(x => x.Principal), items.Sum(x => x.Interest), loan.Currency, items);
    }
}
