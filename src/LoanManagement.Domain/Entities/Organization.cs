using LoanManagement.Domain.Common;

namespace LoanManagement.Domain.Entities;

public sealed class Organization : BaseEntity<string>, IAggregateRoot
{
    private Organization() { } // EF Core

    public Organization(string id, string name)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Organization id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Organization name is required.", nameof(name));

        Id = id;
        Name = name;
        IsActive = true;
    }

    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
