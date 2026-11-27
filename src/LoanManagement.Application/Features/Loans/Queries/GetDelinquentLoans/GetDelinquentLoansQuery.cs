using LoanManagement.Application.Common.Models;
using MediatR;

namespace LoanManagement.Application.Features.Loans.Queries.GetDelinquentLoans;

public sealed record GetDelinquentLoansQuery(string OrganizationId, int MinDaysOverdue, int PageNumber = 1, int PageSize = 20) : IRequest<PaginatedList<DelinquentLoanDto>>;
public sealed record DelinquentLoanDto(string LoanId, string BorrowerName, int DaysOverdue, decimal OverdueAmount, decimal OutstandingBalance, string RiskCategory, DateOnly? LastPaymentDate);
