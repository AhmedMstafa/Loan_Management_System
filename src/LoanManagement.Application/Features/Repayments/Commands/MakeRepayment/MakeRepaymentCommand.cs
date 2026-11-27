using LoanManagement.Application.Common.Models;
using LoanManagement.Domain.Enums;

namespace LoanManagement.Application.Features.Repayments.Commands.MakeRepayment;

public sealed record MakeRepaymentCommand(
    string OrganizationId,
    string LoanId,
    decimal PaymentAmount,
    PaymentMethod PaymentMethod,
    string PaymentReference,
    DateTimeOffset PaidAt) : ICommand<MakeRepaymentResponse>;

public sealed record MakeRepaymentResponse(
    string PaymentId,
    string LoanId,
    decimal AmountPaid,
    decimal PrincipalPortion,
    decimal InterestPortion,
    decimal LateFeePortion,
    decimal NewOutstandingBalance,
    DateTimeOffset ProcessedAt);
