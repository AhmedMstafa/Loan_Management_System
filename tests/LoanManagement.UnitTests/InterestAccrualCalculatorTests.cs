using FluentAssertions;
using LoanManagement.Domain.Services;

namespace LoanManagement.UnitTests;

public class InterestAccrualCalculatorTests
{
    [Fact]
    public void CalculateDailyAccrual_ShouldUse365DayConvention()
    {
        var accrual = InterestAccrualCalculator.CalculateDailyAccrual(36500m, 10m, 365);

        accrual.Should().Be(10.0000m);
    }

    [Fact]
    public void CalculateDailyAccrual_ShouldRoundToFourDecimals_ToEven()
    {
        var accrual = InterestAccrualCalculator.CalculateDailyAccrual(1000m, 7.3m, 365);

        accrual.Should().Be(decimal.Round(1000m * 0.073m / 365m, 4, MidpointRounding.ToEven));
    }
}
