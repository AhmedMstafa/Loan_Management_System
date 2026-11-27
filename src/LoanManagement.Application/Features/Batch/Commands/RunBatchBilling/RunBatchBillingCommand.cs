using LoanManagement.Application.Common.Models;

namespace LoanManagement.Application.Features.Batch.Commands.RunBatchBilling;

public sealed record RunBatchBillingCommand(string OrganizationId, DateOnly BillingCycleDate) : ICommand<RunBatchBillingResponse>;
public sealed record RunBatchBillingResponse(DateOnly CycleDate, int StatementsGenerated, int InvoicesDispatched, string Status);
