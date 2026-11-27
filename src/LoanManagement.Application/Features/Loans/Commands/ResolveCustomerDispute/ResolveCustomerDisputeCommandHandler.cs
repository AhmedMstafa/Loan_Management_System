using LoanManagement.Application.Common.Exceptions;
using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Features.Loans.Commands.ResolveCustomerDispute;

public sealed class ResolveCustomerDisputeCommandHandler : IRequestHandler<ResolveCustomerDisputeCommand, ResolveCustomerDisputeResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;

    public ResolveCustomerDisputeCommandHandler(IApplicationDbContext db, IDateTimeProvider clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<ResolveCustomerDisputeResponse> Handle(ResolveCustomerDisputeCommand request, CancellationToken cancellationToken)
    {
        var dispute = await _db.Disputes.FirstOrDefaultAsync(x => x.Id == request.DisputeId && x.LoanId == request.LoanId && x.OrganizationId == request.OrganizationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Dispute), request.DisputeId);

        var now = _clock.UtcNow;
        dispute.Resolve(request.ResolvedBy, request.ResolutionType, request.AdjustmentAmount, request.ResolutionNotes, now);
        await _db.SaveChangesAsync(cancellationToken);

        return new ResolveCustomerDisputeResponse(dispute.Id, dispute.Status.ToString(), dispute.AdjustmentAmount ?? 0m, request.ResolvedBy, now);
    }
}
