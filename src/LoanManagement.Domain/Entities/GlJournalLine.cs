using LoanManagement.Domain.Common;

namespace LoanManagement.Domain.Entities;

public sealed class GlJournalLine : BaseEntity<long>
{
    private GlJournalLine() { }

    public GlJournalLine(string journalEntryId, string accountNumber, decimal debit, decimal credit)
    {
        if (string.IsNullOrWhiteSpace(journalEntryId)) throw new ArgumentException("Journal entry id is required.", nameof(journalEntryId));
        if (string.IsNullOrWhiteSpace(accountNumber)) throw new ArgumentException("Account number is required.", nameof(accountNumber));
        if (debit < 0 || credit < 0) throw new ArgumentException("Debit and credit cannot be negative.");

        JournalEntryId = journalEntryId;
        AccountNumber = accountNumber.Trim();
        Debit = decimal.Round(debit, 4, MidpointRounding.ToEven);
        Credit = decimal.Round(credit, 4, MidpointRounding.ToEven);
    }

    public string JournalEntryId { get; private set; } = string.Empty;
    public string AccountNumber { get; private set; } = string.Empty;
    public decimal Debit { get; private set; }
    public decimal Credit { get; private set; }
}
