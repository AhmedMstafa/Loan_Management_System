using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Features.Risk.Queries.ExportPortfolioReport;

public sealed class ExportPortfolioReportQueryHandler : IRequestHandler<ExportPortfolioReportQuery, ExportPortfolioReportResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;
    public ExportPortfolioReportQueryHandler(IApplicationDbContext db, IDateTimeProvider clock) { _db = db; _clock = clock; }

    public async Task<ExportPortfolioReportResponse> Handle(ExportPortfolioReportQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Loans.AsNoTracking().Where(x => x.OrganizationId == request.OrganizationId);
        if (request.FilterStatus.Count > 0)
        {
            var statuses = request.FilterStatus.Select(s => Enum.TryParse<LoanStatus>(s, true, out var st) ? st : (LoanStatus?)null)
                .Where(s => s.HasValue).Select(s => s!.Value).ToList();
            if (statuses.Count > 0) query = query.Where(x => statuses.Contains(x.Status));
        }

        var loans = await query.ToListAsync(cancellationToken);
        var now = _clock.UtcNow;
        var reportId = $"rep_{Guid.NewGuid():N}"[..16];
        var url = $"https://storage.bank.internal/reports/portfolio_{request.AsOfDate:yyyyMMdd}.{(request.Format?.ToLowerInvariant() ?? "csv")}";
        return new ExportPortfolioReportResponse(reportId, url, loans.Count, loans.Sum(x => x.OutstandingPrincipal), now);
    }
}
