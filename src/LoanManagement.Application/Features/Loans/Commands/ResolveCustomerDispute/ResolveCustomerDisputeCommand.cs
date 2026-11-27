using LoanManagement.Application.Common.Models;

namespace LoanManagement.Application.Features.Loans.Commands.ResolveCustomerDispute;

public sealed record ResolveCustomerDisputeCommand(
    string OrganizationId,
    string LoanId,
    string DisputeId,
    string ResolutionType,
    decimal AdjustmentAmount,
    string ResolutionNotes,
    string ResolvedBy) : ICommand<ResolveCustomerDisputeResponse>;

public sealed record ResolveCustomerDisputeResponse(string DisputeId, string Status, decimal AdjustedAmount, string ResolvedBy, DateTimeOffset ResolvedAt);
