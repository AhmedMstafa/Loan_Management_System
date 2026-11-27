using LoanManagement.Application.Common.Exceptions;
using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Features.Loans.Commands.ApplyTopUp;

public sealed class ApplyTopUpCommandHandler : IRequestHandler<ApplyTopUpCommand, ApplyTopUpResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;

    public ApplyTopUpCommandHandler(IApplicationDbContext db, IDateTimeProvider clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<ApplyTopUpResponse> Handle(ApplyTopUpCommand request, CancellationToken cancellationToken)
    {
        var loan = await _db.Loans.FirstOrDefaultAsync(x => x.Id == request.LoanId && x.OrganizationId == request.OrganizationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Loan), request.LoanId);

        var now = _clock.UtcNow;
        loan.ApplyTopUp(request.AdditionalAmount, request.TenorExtensionMonths);
        await _db.SaveChangesAsync(cancellationToken);

        return new ApplyTopUpResponse($"top_{Guid.NewGuid():N}"[..16], loan.Id, "Submitted", loan.OutstandingPrincipal, now);
    }
}
