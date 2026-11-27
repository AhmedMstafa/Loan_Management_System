using LoanManagement.Domain.Enums;

namespace LoanManagement.Domain.Entities;

public sealed class GlAccount
{
    private GlAccount() { }

    public GlAccount(string accountNumber, string accountName, GlAccountCategory category)
    {
        if (string.IsNullOrWhiteSpace(accountNumber)) throw new ArgumentException("Account number is required.", nameof(accountNumber));
        if (string.IsNullOrWhiteSpace(accountName)) throw new ArgumentException("Account name is required.", nameof(accountName));

        AccountNumber = accountNumber.Trim();
        AccountName = accountName.Trim();
        Category = category;
        IsActive = true;
    }

    public string AccountNumber { get; private set; } = string.Empty;
    public string AccountName { get; private set; } = string.Empty;
    public GlAccountCategory Category { get; private set; }
    public bool IsActive { get; private set; }
}
