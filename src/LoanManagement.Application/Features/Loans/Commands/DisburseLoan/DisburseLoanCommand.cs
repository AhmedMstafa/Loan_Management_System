using LoanManagement.Application.Common.Models;

namespace LoanManagement.Application.Features.Loans.Commands.DisburseLoan;

public sealed record DisburseLoanCommand(
    string OrganizationId,
    string LoanId,
    decimal DisbursedAmount,
    string DisbursementMethod,
    string BankCode,
    string AccountNumber,
    string AccountHolderName,
    DateOnly FirstRepaymentDueDate) : ICommand<DisburseLoanResponse>;

public sealed record DisburseLoanResponse(string LoanId, string Status, string DisbursementReference, DateTimeOffset DisbursedAt, DateOnly FirstDueDate);
