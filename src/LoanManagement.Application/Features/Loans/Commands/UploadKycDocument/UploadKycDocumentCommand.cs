using LoanManagement.Application.Common.Models;
using LoanManagement.Domain.Enums;

namespace LoanManagement.Application.Features.Loans.Commands.UploadKycDocument;

public sealed record UploadKycDocumentCommand(
    string OrganizationId,
    string BorrowerId,
    string DocumentType,
    string FileName,
    byte[] Content) : ICommand<UploadKycDocumentResponse>;

public sealed record UploadKycDocumentResponse(string DocumentId, string BorrowerId, string DocumentType, KycStatus VerificationStatus, DateTimeOffset UploadedAt);
