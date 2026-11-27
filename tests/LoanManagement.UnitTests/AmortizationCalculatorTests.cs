using FluentAssertions;
using LoanManagement.Domain.Services;

namespace LoanManagement.UnitTests;

public class AmortizationCalculatorTests
{
    [Fact]
    public void GenerateSchedule_ShouldProduceExpectedInstallmentsAndTotals()
    {
        var schedule = AmortizationCalculator.GenerateSchedule(10000m, 12m, 12, new DateOnly(2026, 1, 1));

        schedule.Should().HaveCount(12);
        schedule.Sum(x => x.PrincipalDue).Should().Be(10000m);
        schedule.Sum(x => x.InterestDue).Should().BeGreaterThan(0m);
        schedule.Last().RemainingPrincipal.Should().Be(0m);
        schedule.Select(x => x.InstallmentNumber).Should().BeInAscendingOrder();
    }

    [Fact]
    public void GenerateSchedule_ShouldHandleDifferentTenorAndRounding()
    {
        var schedule = AmortizationCalculator.GenerateSchedule(25000m, 8.5m, 24, new DateOnly(2026, 2, 1));

        schedule.Should().HaveCount(24);
        schedule.Sum(x => x.PrincipalDue).Should().Be(25000m);
        schedule.All(x => x.TotalDue == decimal.Round(x.PrincipalDue + x.InterestDue, 2, MidpointRounding.ToEven)).Should().BeTrue();
        schedule.Last().RemainingPrincipal.Should().Be(0m);
    }
}
