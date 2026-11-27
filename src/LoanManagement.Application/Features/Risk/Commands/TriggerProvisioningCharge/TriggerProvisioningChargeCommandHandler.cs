using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Domain.Entities;
using LoanManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Features.Risk.Commands.TriggerProvisioningCharge;

public sealed class TriggerProvisioningChargeCommandHandler : IRequestHandler<TriggerProvisioningChargeCommand, TriggerProvisioningChargeResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;
    public TriggerProvisioningChargeCommandHandler(IApplicationDbContext db, IDateTimeProvider clock) { _db = db; _clock = clock; }

    public async Task<TriggerProvisioningChargeResponse> Handle(TriggerProvisioningChargeCommand request, CancellationToken cancellationToken)
    {
        var impairedLoans = await _db.Loans.Where(x => x.OrganizationId == request.OrganizationId && (x.Status == LoanStatus.Delinquent || x.Status == LoanStatus.Defaulted)).ToListAsync(cancellationToken);
        var totalProvision = decimal.Round(impairedLoans.Sum(x => x.OutstandingPrincipal * 0.10m), 4, MidpointRounding.ToEven); // simple 10% ECL placeholder

        var now = _clock.UtcNow;
        var journalId = $"GL_{Guid.NewGuid():N}"[..16];
        if (totalProvision > 0)
        {
            var entry = new GlJournalEntry(journalId, request.OrganizationId, DateOnly.FromDateTime(now.UtcDateTime), $"Provisioning charge {request.Period} ({request.ProvisioningModel})");
            entry.AddLine("5100-ProvisionExpense", totalProvision, 0m);
            entry.AddLine("1200-LoansReceivable", 0m, totalProvision);
            entry.ValidateDoubleEntry();
            entry.MarkCreated(now);
            _db.GlJournalEntries.Add(entry);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return new TriggerProvisioningChargeResponse(request.Period, impairedLoans.Count, totalProvision, journalId, now);
    }
}
