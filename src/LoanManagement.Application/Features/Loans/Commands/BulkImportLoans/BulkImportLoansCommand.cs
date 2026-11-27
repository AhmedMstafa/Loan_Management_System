using LoanManagement.Application.Common.Models;

namespace LoanManagement.Application.Features.Loans.Commands.BulkImportLoans;

public sealed record BulkImportLoansCommand(string OrganizationId, string FileName, byte[] Content) : ICommand<BulkImportLoansResponse>;
public sealed record BulkImportLoansResponse(string ImportJobId, string Status, string StatusCheckUrl, DateTimeOffset QueuedAt);
