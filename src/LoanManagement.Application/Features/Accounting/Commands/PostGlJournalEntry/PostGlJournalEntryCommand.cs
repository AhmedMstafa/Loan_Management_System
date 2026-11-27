using LoanManagement.Application.Common.Models;

namespace LoanManagement.Application.Features.Accounting.Commands.PostGlJournalEntry;

public sealed record PostGlJournalEntryCommand(string OrganizationId, DateOnly AccountingDate, string Memo, IReadOnlyList<GlJournalLineDto> Lines) : ICommand<PostGlJournalEntryResponse>;
public sealed record GlJournalLineDto(string AccountNumber, decimal Debit, decimal Credit);
public sealed record PostGlJournalEntryResponse(string JournalEntryId, string Status, decimal TotalDebit, decimal TotalCredit, DateTimeOffset PostedAt);
