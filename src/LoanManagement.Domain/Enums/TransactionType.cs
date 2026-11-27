namespace LoanManagement.Domain.Enums;

public enum TransactionType
{
    Disbursement = 0,
    PrincipalPayment = 1,
    InterestPayment = 2,
    Fee = 3,
    Provision = 4,
    Accrual = 5,
    Adjustment = 6
}
