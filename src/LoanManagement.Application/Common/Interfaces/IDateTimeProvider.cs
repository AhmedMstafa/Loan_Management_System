namespace LoanManagement.Application.Common.Interfaces;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
    DateOnly UtcToday { get; }
}
