using LoanManagement.Domain.Common;
using LoanManagement.Domain.Enums;

namespace LoanManagement.Domain.Entities;

public sealed class Borrower : BaseEntity<string>, IAggregateRoot
{
    private Borrower() { }

    public Borrower(
        string id,
        string organizationId,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string nationalIdHash,
        decimal monthlyIncome)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Borrower id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(organizationId)) throw new ArgumentException("Organization id is required.", nameof(organizationId));
        if (monthlyIncome < 0) throw new ArgumentOutOfRangeException(nameof(monthlyIncome), "Monthly income cannot be negative.");

        Id = id;
        OrganizationId = organizationId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        NationalIdHash = nationalIdHash;
        MonthlyIncome = decimal.Round(monthlyIncome, 4, MidpointRounding.ToEven);
        KycStatus = KycStatus.Pending;
    }

    public string OrganizationId { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string NationalIdHash { get; private set; } = string.Empty;
    public KycStatus KycStatus { get; private set; }
    public decimal MonthlyIncome { get; private set; }

    public void UpdateKycStatus(KycStatus status) => KycStatus = status;
}
