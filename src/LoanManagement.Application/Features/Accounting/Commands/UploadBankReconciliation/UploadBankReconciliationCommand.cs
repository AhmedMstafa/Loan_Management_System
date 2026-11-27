using LoanManagement.Application.Common.Models;

namespace LoanManagement.Application.Features.Accounting.Commands.UploadBankReconciliation;

public sealed record UploadBankReconciliationCommand(string OrganizationId, DateOnly BankStatementDate, IReadOnlyList<BankReconTxnDto> Transactions) : ICommand<UploadBankReconciliationResponse>;
public sealed record BankReconTxnDto(string BankTxnId, decimal Amount, string Reference, DateOnly Date);
public sealed record UploadBankReconciliationResponse(int TotalProcessed, int MatchedCount, int UnmatchedCount, string Status);
