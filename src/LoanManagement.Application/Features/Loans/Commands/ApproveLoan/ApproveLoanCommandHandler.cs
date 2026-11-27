using LoanManagement.Application.Common.Exceptions;
using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Domain.Entities;
using LoanManagement.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Features.Loans.Commands.ApproveLoan;

public sealed class ApproveLoanCommandHandler : IRequestHandler<ApproveLoanCommand, ApproveLoanResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;

    public ApproveLoanCommandHandler(IApplicationDbContext db, IDateTimeProvider clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<ApproveLoanResponse> Handle(ApproveLoanCommand request, CancellationToken cancellationToken)
    {
        var loan = await _db.Loans
            .Include(x => x.RepaymentSchedule)
            .FirstOrDefaultAsync(x => x.Id == request.LoanId && x.OrganizationId == request.OrganizationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Loan), request.LoanId);

        var now = _clock.UtcNow;
        loan.Approve(request.ApprovedAmount, request.ApprovedInterestRate, request.TenorMonths, request.ApprovedBy, now);

        var firstDueDate = request.ScheduledDisbursementDate.AddMonths(1);
        var schedule = AmortizationCalculator.GenerateScheduleItems(
            loan.Id,
            request.ApprovedAmount,
            request.ApprovedInterestRate,
            request.TenorMonths,
            firstDueDate);

        loan.ReplaceRepaymentSchedule(schedule);
        await _db.SaveChangesAsync(cancellationToken);

        return new ApproveLoanResponse(loan.Id, loan.Status.ToString(), loan.ApprovedAmount!.Value, request.ApprovedBy, now);
    }
}
