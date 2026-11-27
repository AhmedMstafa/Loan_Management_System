using FluentAssertions;
using LoanManagement.Domain.Entities;
using LoanManagement.Domain.Exceptions;

namespace LoanManagement.UnitTests;

public class GlJournalEntryTests
{
    [Fact]
    public void ValidateDoubleEntry_ShouldPass_WhenDebitsEqualCredits()
    {
        var entry = new GlJournalEntry("GL1", "org1", DateOnly.FromDateTime(DateTime.UtcNow), "Test");
        entry.AddLine("1100-Cash", 100m, 0m);
        entry.AddLine("4100-InterestIncome", 0m, 100m);

        var act = () => entry.ValidateDoubleEntry();

        act.Should().NotThrow();
        entry.TotalDebit.Should().Be(entry.TotalCredit);
    }

    [Fact]
    public void ValidateDoubleEntry_ShouldThrow_WhenUnbalanced()
    {
        var entry = new GlJournalEntry("GL2", "org1", DateOnly.FromDateTime(DateTime.UtcNow), "Unbalanced");
        entry.AddLine("1100-Cash", 100m, 0m);
        entry.AddLine("4100-InterestIncome", 0m, 90m);

        var act = () => entry.ValidateDoubleEntry();

        act.Should().Throw<DomainValidationException>();
    }
}
