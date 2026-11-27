using LoanManagement.Application.Common.Exceptions;
using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Infrastructure.Services;

public sealed class IdempotencyService : IIdempotencyService
{
    private readonly IApplicationDbContext _db;

    public IdempotencyService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IdempotencyRecordResult?> GetAsync(string organizationId, string idempotencyKey, string requestHash, CancellationToken cancellationToken = default)
    {
        var record = await _db.IdempotencyRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == idempotencyKey && x.OrganizationId == organizationId, cancellationToken);

        if (record is null)
            return null;

        if (record.ExpiresAt <= DateTimeOffset.UtcNow)
            return null;

        if (!string.Equals(record.RequestHash, requestHash, StringComparison.Ordinal))
            throw new IdempotencyConflictException(idempotencyKey);

        return new IdempotencyRecordResult(record.StatusCode, record.ResponsePayload);
    }

    public async Task StoreAsync(string organizationId, string idempotencyKey, string requestHash, int statusCode, string responsePayload, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var existing = await _db.IdempotencyRecords
            .FirstOrDefaultAsync(x => x.Id == idempotencyKey && x.OrganizationId == organizationId, cancellationToken);

        if (existing is not null)
        {
            if (!string.Equals(existing.RequestHash, requestHash, StringComparison.Ordinal))
                throw new IdempotencyConflictException(idempotencyKey);
            return;
        }

        var record = new IdempotencyRecord(idempotencyKey, organizationId, requestHash, statusCode, responsePayload, now, now.Add(ttl));
        _db.IdempotencyRecords.Add(record);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
