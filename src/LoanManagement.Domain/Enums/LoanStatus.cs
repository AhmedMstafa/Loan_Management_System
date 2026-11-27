namespace LoanManagement.Domain.Enums;

public enum LoanStatus
{
    Draft = 0,
    Submitted = 1,
    Underwriting = 2,
    Approved = 3,
    Disbursed = 4,
    Active = 5,
    Delinquent = 6,
    Closed = 7,
    Defaulted = 8,
    WrittenOff = 9
}
