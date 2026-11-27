namespace LoanManagement.Application.Common.Interfaces;

public sealed record IdempotencyRecordResult(int StatusCode, string ResponsePayload);

public interface IIdempotencyService
{
    Task<IdempotencyRecordResult?> GetAsync(string organizationId, string idempotencyKey, string requestHash, CancellationToken cancellationToken = default);
    Task StoreAsync(string organizationId, string idempotencyKey, string requestHash, int statusCode, string responsePayload, TimeSpan ttl, CancellationToken cancellationToken = default);
}
