using LoanManagement.Application.Features.Audit.Queries.GetAuditLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagement.API.Controllers;

public sealed class AuditController : BaseApiController
{
    [HttpGet("audit/logs")]
    [Authorize(Roles = "Security,Compliance,Admin")]
    public async Task<IActionResult> GetLogs([FromQuery] string? entity, [FromQuery] string? entityId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
        => Ok(await Mediator.Send(new GetAuditLogsQuery(GetOrganizationId(), entity, entityId, pageNumber, pageSize), ct));
}
