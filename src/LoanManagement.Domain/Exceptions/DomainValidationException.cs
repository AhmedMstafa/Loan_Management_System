namespace LoanManagement.Domain.Exceptions;

public sealed class DomainValidationException : Exception
{
    public DomainValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]> { ["Domain"] = [message] };
    }

    public DomainValidationException(string field, string message) : base(message)
    {
        Errors = new Dictionary<string, string[]> { [field] = [message] };
    }

    public DomainValidationException(IDictionary<string, string[]> errors)
        : base("One or more domain validation errors occurred.")
    {
        Errors = errors;
    }

    public IDictionary<string, string[]> Errors { get; }
}
