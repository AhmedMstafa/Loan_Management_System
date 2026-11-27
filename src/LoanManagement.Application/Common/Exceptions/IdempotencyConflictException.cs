namespace LoanManagement.Application.Common.Exceptions;

public sealed class IdempotencyConflictException : Exception
{
    public IdempotencyConflictException(string key)
        : base($"An operation with idempotency key '{key}' already exists with a different request payload.")
    {
        Key = key;
    }

    public string Key { get; }
}
