using LoanManagement.Domain.Common;
using LoanManagement.Domain.Exceptions;

namespace LoanManagement.Domain.Entities;

public sealed class GlJournalEntry : BaseEntity<string>, IAggregateRoot
{
    private readonly List<GlJournalLine> _lines = [];

    private GlJournalEntry() { }

    public GlJournalEntry(string id, string organizationId, DateOnly accountingDate, string memo)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Journal entry id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(organizationId)) throw new ArgumentException("Organization id is required.", nameof(organizationId));

        Id = id;
        OrganizationId = organizationId;
        AccountingDate = accountingDate;
        Memo = memo?.Trim() ?? string.Empty;
        Status = "Posted";
    }

    public string OrganizationId { get; private set; } = string.Empty;
    public DateOnly AccountingDate { get; private set; }
    public string Memo { get; private set; } = string.Empty;
    public string Status { get; private set; } = "Posted";

    public IReadOnlyCollection<GlJournalLine> Lines => _lines.AsReadOnly();
    public decimal TotalDebit => decimal.Round(_lines.Sum(l => l.Debit), 4, MidpointRounding.ToEven);
    public decimal TotalCredit => decimal.Round(_lines.Sum(l => l.Credit), 4, MidpointRounding.ToEven);

    public void AddLine(string accountNumber, decimal debit, decimal credit)
    {
        _lines.Add(new GlJournalLine(Id, accountNumber, debit, credit));
    }

    public void ValidateDoubleEntry()
    {
        if (_lines.Count < 2)
            throw new DomainValidationException("Journal entry must have at least two lines.");
        if (TotalDebit != TotalCredit)
            throw new DomainValidationException(
                $"Journal entry is not balanced. Debit={TotalDebit}, Credit={TotalCredit}.");
        if (_lines.Any(l => l.Debit < 0 || l.Credit < 0))
            throw new DomainValidationException("Journal line amounts cannot be negative.");
        if (_lines.Any(l => l.Debit > 0 && l.Credit > 0))
            throw new DomainValidationException("A journal line cannot have both debit and credit greater than zero.");
    }
}
