using LoanManagement.Domain.Common;

namespace LoanManagement.Domain.Entities;

public sealed class User : BaseEntity<string>, IAggregateRoot
{
    private User() { }

    public User(string id, string organizationId, string email, string passwordHash, string fullName, string role, bool isActive = true)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("User id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(organizationId)) throw new ArgumentException("Organization id is required.", nameof(organizationId));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        if (string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("Full name is required.", nameof(fullName));
        if (string.IsNullOrWhiteSpace(role)) throw new ArgumentException("Role is required.", nameof(role));

        Id = id;
        OrganizationId = organizationId;
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        FullName = fullName.Trim();
        Role = role.Trim();
        IsActive = isActive;
    }

    public string OrganizationId { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
