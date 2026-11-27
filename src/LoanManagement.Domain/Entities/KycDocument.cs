using LoanManagement.Domain.Common;
using LoanManagement.Domain.Enums;

namespace LoanManagement.Domain.Entities;

public sealed class KycDocument : BaseEntity<string>
{
    private KycDocument() { }

    public KycDocument(
        string id,
        string organizationId,
        string borrowerId,
        string documentType,
        string storagePath,
        DateTimeOffset uploadedAt)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Document id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(borrowerId)) throw new ArgumentException("Borrower id is required.", nameof(borrowerId));

        Id = id;
        OrganizationId = organizationId;
        BorrowerId = borrowerId;
        DocumentType = documentType?.Trim() ?? string.Empty;
        StoragePath = storagePath?.Trim() ?? string.Empty;
        VerificationStatus = KycStatus.Pending;
        UploadedAt = uploadedAt;
    }

    public string OrganizationId { get; private set; } = string.Empty;
    public string BorrowerId { get; private set; } = string.Empty;
    public string DocumentType { get; private set; } = string.Empty;
    public string StoragePath { get; private set; } = string.Empty;
    public KycStatus VerificationStatus { get; private set; }
    public DateTimeOffset UploadedAt { get; private set; }
    public DateTimeOffset? VerifiedAt { get; private set; }
    public string? VerifiedBy { get; private set; }

    public void MarkVerified(string verifiedBy, DateTimeOffset verifiedAt)
    {
        VerificationStatus = KycStatus.Verified;
        VerifiedBy = verifiedBy;
        VerifiedAt = verifiedAt;
    }

    public void MarkRejected(string verifiedBy, DateTimeOffset verifiedAt)
    {
        VerificationStatus = KycStatus.Rejected;
        VerifiedBy = verifiedBy;
        VerifiedAt = verifiedAt;
    }
}
