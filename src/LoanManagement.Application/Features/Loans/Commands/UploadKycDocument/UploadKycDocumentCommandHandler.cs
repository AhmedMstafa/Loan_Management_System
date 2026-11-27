using LoanManagement.Application.Common.Exceptions;
using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Features.Loans.Commands.UploadKycDocument;

public sealed class UploadKycDocumentCommandHandler : IRequestHandler<UploadKycDocumentCommand, UploadKycDocumentResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly IDocumentStorageService _storage;
    private readonly IDateTimeProvider _clock;

    public UploadKycDocumentCommandHandler(IApplicationDbContext db, IDocumentStorageService storage, IDateTimeProvider clock)
    {
        _db = db;
        _storage = storage;
        _clock = clock;
    }

    public async Task<UploadKycDocumentResponse> Handle(UploadKycDocumentCommand request, CancellationToken cancellationToken)
    {
        var borrower = await _db.Borrowers.FirstOrDefaultAsync(x => x.Id == request.BorrowerId && x.OrganizationId == request.OrganizationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Borrower), request.BorrowerId);

        await using var stream = new MemoryStream(request.Content);
        var stored = await _storage.SaveAsync(request.OrganizationId, borrower.Id, request.DocumentType, request.FileName, stream, cancellationToken);

        var now = _clock.UtcNow;
        var doc = new KycDocument($"doc_{Guid.NewGuid():N}"[..16], request.OrganizationId, borrower.Id, request.DocumentType, stored.StoragePath, now);
        doc.MarkCreated(now);
        _db.KycDocuments.Add(doc);
        await _db.SaveChangesAsync(cancellationToken);

        return new UploadKycDocumentResponse(doc.Id, borrower.Id, doc.DocumentType, doc.VerificationStatus, doc.UploadedAt);
    }
}
