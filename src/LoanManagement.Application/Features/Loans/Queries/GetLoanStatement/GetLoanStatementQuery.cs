using MediatR;

namespace LoanManagement.Application.Features.Loans.Queries.GetLoanStatement;

public sealed record GetLoanStatementQuery(string OrganizationId, string LoanId, DateOnly From, DateOnly To, string Format) : IRequest<GetLoanStatementResponse>;
public sealed record GetLoanStatementResponse(string LoanId, string StatementUrl, decimal OpeningBalance, decimal ClosingBalance, decimal TotalInterestPaid, decimal TotalPrincipalPaid);
