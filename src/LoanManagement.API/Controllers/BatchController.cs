using LoanManagement.Application.Features.Batch.Commands.GenerateDailyAccruals;
using LoanManagement.Application.Features.Batch.Commands.RunBatchBilling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagement.API.Controllers;

public sealed class BatchController : BaseApiController
{
    [HttpPost("batch/interest-accruals")]
    [Authorize(Roles = "System,Admin")]
    public async Task<ActionResult<GenerateDailyAccrualsResponse>> GenerateAccruals([FromBody] GenerateDailyAccrualsRequest request, CancellationToken ct)
        => Ok(await Mediator.Send(new GenerateDailyAccrualsCommand(GetOrganizationId(), request.AccrualDate), ct));

    [HttpPost("batch/billing")]
    [Authorize(Roles = "Scheduler,Admin")]
    public async Task<ActionResult<RunBatchBillingResponse>> RunBilling([FromBody] RunBatchBillingRequest request, CancellationToken ct)
        => Ok(await Mediator.Send(new RunBatchBillingCommand(GetOrganizationId(), request.BillingCycleDate), ct));
}

public sealed record GenerateDailyAccrualsRequest(DateOnly AccrualDate);
public sealed record RunBatchBillingRequest(DateOnly BillingCycleDate);
