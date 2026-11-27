using LoanManagement.Application.Common.Models;
using MediatR;

namespace LoanManagement.Application.Features.Audit.Queries.GetAuditLogs;

public sealed record GetAuditLogsQuery(string OrganizationId, string? Entity, string? EntityId, int PageNumber = 1, int PageSize = 50) : IRequest<PaginatedList<AuditLogDto>>;
public sealed record AuditLogDto(long Id, DateTimeOffset Timestamp, string Action, string ActorId, string ActorRole, string Entity, string EntityId, string? IpAddress, string? OldValues, string? NewValues);
