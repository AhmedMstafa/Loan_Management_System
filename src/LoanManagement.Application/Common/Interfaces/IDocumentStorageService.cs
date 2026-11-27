namespace LoanManagement.Application.Common.Interfaces;

public sealed record StoredDocument(string StoragePath, string FileName, long SizeBytes);

public interface IDocumentStorageService
{
    Task<StoredDocument> SaveAsync(string organizationId, string borrowerId, string documentType, string fileName, Stream content, CancellationToken cancellationToken = default);
}
