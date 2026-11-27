using LoanManagement.Application.Common.Models;
using MediatR;

namespace LoanManagement.Application.Features.Loans.Queries.GetPaymentSchedule;

public sealed record GetPaymentScheduleQuery(string OrganizationId, string LoanId) : IRequest<GetPaymentScheduleResponse>;

public sealed record GetPaymentScheduleResponse(string LoanId, decimal TotalPrincipal, decimal TotalInterest, string Currency, IReadOnlyList<PaymentScheduleItemDto> Installments);
public sealed record PaymentScheduleItemDto(int InstallmentNumber, DateOnly DueDate, decimal Principal, decimal Interest, decimal TotalDue, decimal RemainingBalance, string Status);
