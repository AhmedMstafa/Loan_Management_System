using LoanManagement.Application.Common.Models;

namespace LoanManagement.Application.Features.Batch.Commands.GenerateDailyAccruals;

public sealed record GenerateDailyAccrualsCommand(string OrganizationId, DateOnly AccrualDate) : ICommand<GenerateDailyAccrualsResponse>;
public sealed record GenerateDailyAccrualsResponse(int ProcessedLoansCount, decimal TotalAccruedInterest, string BatchRunId, DateTimeOffset CompletedAt);
