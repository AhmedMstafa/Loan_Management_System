using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Features.Batch.Commands.RunBatchBilling;

public sealed class RunBatchBillingCommandHandler : IRequestHandler<RunBatchBillingCommand, RunBatchBillingResponse>
{
    private readonly IApplicationDbContext _db;
    public RunBatchBillingCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<RunBatchBillingResponse> Handle(RunBatchBillingCommand request, CancellationToken cancellationToken)
    {
        var count = await _db.Loans.AsNoTracking()
            .CountAsync(x => x.OrganizationId == request.OrganizationId && (x.Status == LoanStatus.Active || x.Status == LoanStatus.Delinquent || x.Status == LoanStatus.Disbursed), cancellationToken);
        // Placeholder billing: in a full system this would generate and dispatch statements.
        return new RunBatchBillingResponse(request.BillingCycleDate, count, count, "Completed");
    }
}
