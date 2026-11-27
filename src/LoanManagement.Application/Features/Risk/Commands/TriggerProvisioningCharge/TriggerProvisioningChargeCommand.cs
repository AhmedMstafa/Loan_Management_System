using LoanManagement.Application.Common.Models;

namespace LoanManagement.Application.Features.Risk.Commands.TriggerProvisioningCharge;

public sealed record TriggerProvisioningChargeCommand(string OrganizationId, string Period, string ProvisioningModel) : ICommand<TriggerProvisioningChargeResponse>;
public sealed record TriggerProvisioningChargeResponse(string Period, int TotalImpairedLoans, decimal TotalProvisionCharge, string PostedGlJournalEntryId, DateTimeOffset ProcessedAt);
