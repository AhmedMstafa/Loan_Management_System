using LoanManagement.Domain.Entities;

namespace LoanManagement.Domain.Services;

public sealed record AmortizationInstallment(
    int InstallmentNumber,
    DateOnly DueDate,
    decimal PrincipalDue,
    decimal InterestDue,
    decimal TotalDue,
    decimal RemainingPrincipal);

public static class AmortizationCalculator
{
    /// <summary>
    /// Generates an equal monthly installment (EMI) amortization schedule.
    /// All monetary values are rounded to 2 decimal places using MidpointRounding.ToEven.
    /// The final installment is adjusted to fully settle the remaining principal.
    /// </summary>
    public static IReadOnlyList<AmortizationInstallment> GenerateSchedule(
        decimal principal,
        decimal annualInterestRatePercent,
        int tenorMonths,
        DateOnly firstDueDate)
    {
        if (principal <= 0) throw new ArgumentOutOfRangeException(nameof(principal), "Principal must be greater than zero.");
        if (annualInterestRatePercent < 0) throw new ArgumentOutOfRangeException(nameof(annualInterestRatePercent), "Interest rate cannot be negative.");
        if (tenorMonths <= 0) throw new ArgumentOutOfRangeException(nameof(tenorMonths), "Tenor must be greater than zero.");

        var monthlyRate = annualInterestRatePercent / 12m / 100m;
        var emi = CalculateEmi(principal, monthlyRate, tenorMonths);

        var schedule = new List<AmortizationInstallment>(tenorMonths);
        var remaining = decimal.Round(principal, 2, MidpointRounding.ToEven);

        for (var i = 1; i <= tenorMonths; i++)
        {
            var dueDate = firstDueDate.AddMonths(i - 1);
            decimal interest;
            decimal principalComponent;
            decimal remainingAfter;

            if (i == tenorMonths)
            {
                interest = decimal.Round(remaining * monthlyRate, 2, MidpointRounding.ToEven);
                principalComponent = decimal.Round(remaining, 2, MidpointRounding.ToEven);
                var total = decimal.Round(principalComponent + interest, 2, MidpointRounding.ToEven);
                remainingAfter = 0m;
                schedule.Add(new AmortizationInstallment(i, dueDate, principalComponent, interest, total, remainingAfter));
                break;
            }

            interest = decimal.Round(remaining * monthlyRate, 2, MidpointRounding.ToEven);
            principalComponent = decimal.Round(emi - interest, 2, MidpointRounding.ToEven);

            if (principalComponent > remaining)
                principalComponent = remaining;

            remainingAfter = decimal.Round(remaining - principalComponent, 2, MidpointRounding.ToEven);
            var installmentTotal = decimal.Round(principalComponent + interest, 2, MidpointRounding.ToEven);
            schedule.Add(new AmortizationInstallment(i, dueDate, principalComponent, interest, installmentTotal, remainingAfter));
            remaining = remainingAfter;
        }

        return schedule;
    }

    public static decimal CalculateEmi(decimal principal, decimal monthlyRate, int tenorMonths)
    {
        if (monthlyRate == 0m)
            return decimal.Round(principal / tenorMonths, 2, MidpointRounding.ToEven);

        var factor = Pow(1m + monthlyRate, tenorMonths);
        var emi = principal * monthlyRate * factor / (factor - 1m);
        return decimal.Round(emi, 2, MidpointRounding.ToEven);
    }

    public static IReadOnlyList<RepaymentScheduleItem> GenerateScheduleItems(
        string loanId,
        decimal principal,
        decimal annualInterestRatePercent,
        int tenorMonths,
        DateOnly firstDueDate)
    {
        var installments = GenerateSchedule(principal, annualInterestRatePercent, tenorMonths, firstDueDate);
        return installments.Select(i => new RepaymentScheduleItem(
            loanId,
            i.InstallmentNumber,
            i.DueDate,
            i.PrincipalDue,
            i.InterestDue,
            i.RemainingPrincipal)).ToList();
    }

    private static decimal Pow(decimal baseValue, int exponent)
    {
        if (exponent < 0) throw new ArgumentOutOfRangeException(nameof(exponent));
        decimal result = 1m;
        for (var i = 0; i < exponent; i++)
            result *= baseValue;
        return result;
    }
}
