using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Features.Audit.Queries.GetAuditLogs;

public sealed class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, PaginatedList<AuditLogDto>>
{
    private readonly IApplicationDbContext _db;
    public GetAuditLogsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<PaginatedList<AuditLogDto>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.AuditLogs.AsNoTracking().Where(x => x.OrganizationId == request.OrganizationId);
        if (!string.IsNullOrWhiteSpace(request.Entity)) query = query.Where(x => x.Entity == request.Entity);
        if (!string.IsNullOrWhiteSpace(request.EntityId)) query = query.Where(x => x.EntityId == request.EntityId);
        query = query.OrderByDescending(x => x.Timestamp);

        var projected = query.Select(x => new AuditLogDto(x.Id, x.Timestamp, x.Action, x.ActorId, x.ActorRole, x.Entity, x.EntityId, x.IpAddress, x.OldValues, x.NewValues));
        return await PaginatedList<AuditLogDto>.CreateAsync(projected, request.PageNumber, request.PageSize, cancellationToken);
    }
}
