namespace LoanManagement.Domain.Services;

public static class InterestAccrualCalculator
{
    /// <summary>
    /// Calculates daily interest accrual using the formula:
    /// Daily Accrual = Outstanding Balance * (Annual Rate / 100) / 365
    /// Returns value rounded to 4 decimal places using MidpointRounding.ToEven.
    /// </summary>
    public static decimal CalculateDailyAccrual(decimal outstandingBalance, decimal annualInterestRatePercent, int daysInYear = 365)
    {
        if (outstandingBalance < 0)
            throw new ArgumentOutOfRangeException(nameof(outstandingBalance), "Outstanding balance cannot be negative.");
        if (annualInterestRatePercent < 0)
            throw new ArgumentOutOfRangeException(nameof(annualInterestRatePercent), "Annual interest rate cannot be negative.");
        if (daysInYear <= 0)
            throw new ArgumentOutOfRangeException(nameof(daysInYear), "Days in year must be greater than zero.");
        if (outstandingBalance == 0m || annualInterestRatePercent == 0m)
            return 0m;

        var dailyRate = annualInterestRatePercent / 100m / daysInYear;
        var accrual = outstandingBalance * dailyRate;
        return decimal.Round(accrual, 4, MidpointRounding.ToEven);
    }

    public static decimal CalculateAccrualForPeriod(decimal outstandingBalance, decimal annualInterestRatePercent, int days, int daysInYear = 365)
    {
        if (days < 0) throw new ArgumentOutOfRangeException(nameof(days), "Days cannot be negative.");
        var daily = CalculateDailyAccrual(outstandingBalance, annualInterestRatePercent, daysInYear);
        return decimal.Round(daily * days, 4, MidpointRounding.ToEven);
    }
}
