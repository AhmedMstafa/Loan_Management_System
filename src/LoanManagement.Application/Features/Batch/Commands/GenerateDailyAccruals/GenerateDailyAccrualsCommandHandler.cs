using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Domain.Enums;
using LoanManagement.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Features.Batch.Commands.GenerateDailyAccruals;

public sealed class GenerateDailyAccrualsCommandHandler : IRequestHandler<GenerateDailyAccrualsCommand, GenerateDailyAccrualsResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;
    public GenerateDailyAccrualsCommandHandler(IApplicationDbContext db, IDateTimeProvider clock) { _db = db; _clock = clock; }

    public async Task<GenerateDailyAccrualsResponse> Handle(GenerateDailyAccrualsCommand request, CancellationToken cancellationToken)
    {
        var loans = await _db.Loans.Where(x => x.OrganizationId == request.OrganizationId && (x.Status == LoanStatus.Active || x.Status == LoanStatus.Disbursed || x.Status == LoanStatus.Delinquent))
            .ToListAsync(cancellationToken);

        decimal total = 0m;
        foreach (var loan in loans)
        {
            var accrual = InterestAccrualCalculator.CalculateDailyAccrual(loan.OutstandingPrincipal, loan.InterestRate);
            if (accrual <= 0m) continue;
            loan.AccrueInterest(accrual);
            total = decimal.Round(total + accrual, 4, MidpointRounding.ToEven);
        }

        await _db.SaveChangesAsync(cancellationToken);
        var now = _clock.UtcNow;
        return new GenerateDailyAccrualsResponse(loans.Count, total, $"batch_accrual_{request.AccrualDate:yyyyMMdd}", now);
    }
}
