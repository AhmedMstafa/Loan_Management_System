using LoanManagement.Application.Common.Interfaces;
using MediatR;

namespace LoanManagement.Application.Features.Loans.Commands.BulkImportLoans;

public sealed class BulkImportLoansCommandHandler : IRequestHandler<BulkImportLoansCommand, BulkImportLoansResponse>
{
    private readonly IDateTimeProvider _clock;
    public BulkImportLoansCommandHandler(IDateTimeProvider clock) => _clock = clock;

    public Task<BulkImportLoansResponse> Handle(BulkImportLoansCommand request, CancellationToken cancellationToken)
    {
        // Placeholder: real implementation would parse file and enqueue background job.
        var now = _clock.UtcNow;
        var jobId = $"job_imp_{Guid.NewGuid():N}"[..16];
        return Task.FromResult(new BulkImportLoansResponse(jobId, "Queued", $"/api/v1/jobs/{jobId}", now));
    }
}
