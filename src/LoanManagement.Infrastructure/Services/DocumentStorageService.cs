using LoanManagement.Application.Common.Interfaces;

namespace LoanManagement.Infrastructure.Services;

public sealed class DocumentStorageService : IDocumentStorageService
{
    private readonly string _rootPath;

    public DocumentStorageService(string? rootPath = null)
    {
        _rootPath = rootPath ?? Path.Combine(AppContext.BaseDirectory, "storage", "kyc");
        Directory.CreateDirectory(_rootPath);
    }

    public async Task<StoredDocument> SaveAsync(string organizationId, string borrowerId, string documentType, string fileName, Stream content, CancellationToken cancellationToken = default)
    {
        var safeOrg = Sanitize(organizationId);
        var safeBorrower = Sanitize(borrowerId);
        var safeType = Sanitize(documentType);
        var ext = Path.GetExtension(fileName);
        var storedName = $"{Guid.NewGuid():N}{ext}";
        var folder = Path.Combine(_rootPath, safeOrg, safeBorrower, safeType);
        Directory.CreateDirectory(folder);

        var fullPath = Path.Combine(folder, storedName);
        await using var fileStream = File.Create(fullPath);
        await content.CopyToAsync(fileStream, cancellationToken);

        var relativePath = Path.GetRelativePath(_rootPath, fullPath).Replace('\\', '/');
        return new StoredDocument(relativePath, fileName, fileStream.Length);
    }

    private static string Sanitize(string value)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var cleaned = new string((value ?? string.Empty).Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray());
        return string.IsNullOrWhiteSpace(cleaned) ? "unknown" : cleaned;
    }
}
