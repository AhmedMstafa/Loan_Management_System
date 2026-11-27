using MediatR;

namespace LoanManagement.Application.Features.Risk.Queries.ExportPortfolioReport;

public sealed record ExportPortfolioReportQuery(string OrganizationId, string Format, DateOnly AsOfDate, IReadOnlyList<string> FilterStatus) : IRequest<ExportPortfolioReportResponse>;
public sealed record ExportPortfolioReportResponse(string ReportId, string DownloadUrl, int TotalLoans, decimal TotalOutstandingPrincipal, DateTimeOffset GeneratedAt);
