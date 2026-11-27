namespace LoanManagement.Domain.Exceptions;

public sealed class InvalidLoanStateException : Exception
{
    public InvalidLoanStateException(string loanId, string currentStatus, string attemptedAction)
        : base($"Loan '{loanId}' in status '{currentStatus}' cannot perform '{attemptedAction}'.")
    {
        LoanId = loanId;
        CurrentStatus = currentStatus;
        AttemptedAction = attemptedAction;
    }

    public string LoanId { get; }
    public string CurrentStatus { get; }
    public string AttemptedAction { get; }
}
