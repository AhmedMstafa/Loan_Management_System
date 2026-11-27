using LoanManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Features.Accounting.Commands.UploadBankReconciliation;

public sealed class UploadBankReconciliationCommandHandler : IRequestHandler<UploadBankReconciliationCommand, UploadBankReconciliationResponse>
{
    private readonly IApplicationDbContext _db;
    public UploadBankReconciliationCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<UploadBankReconciliationResponse> Handle(UploadBankReconciliationCommand request, CancellationToken cancellationToken)
    {
        var refs = request.Transactions.Select(t => t.Reference).Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
        var matchedRefs = await _db.RepaymentPayments.AsNoTracking()
            .Where(p => p.OrganizationId == request.OrganizationId && refs.Contains(p.TransactionReference))
            .Select(p => p.TransactionReference)
            .ToListAsync(cancellationToken);

        var matched = request.Transactions.Count(t => matchedRefs.Contains(t.Reference));
        var total = request.Transactions.Count;
        var unmatched = total - matched;
        return new UploadBankReconciliationResponse(total, matched, unmatched, unmatched == 0 ? "Reconciled" : "PartiallyReconciled");
    }
}
