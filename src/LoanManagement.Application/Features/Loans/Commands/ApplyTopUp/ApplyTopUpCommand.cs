using LoanManagement.Application.Common.Models;

namespace LoanManagement.Application.Features.Loans.Commands.ApplyTopUp;

public sealed record ApplyTopUpCommand(string OrganizationId, string LoanId, decimal AdditionalAmount, int TenorExtensionMonths, string Reason) : ICommand<ApplyTopUpResponse>;
public sealed record ApplyTopUpResponse(string TopUpRequestId, string LoanId, string Status, decimal NewTotalPrincipal, DateTimeOffset SubmittedAt);
