using LoanManagement.Application.Features.Risk.Commands.TriggerProvisioningCharge;
using LoanManagement.Application.Features.Risk.Queries.ExportPortfolioReport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagement.API.Controllers;

public sealed class RiskController : BaseApiController
{
    [HttpPost("reports/portfolio-export")]
    [Authorize(Roles = "RiskAnalyst,Admin")]
    public async Task<ActionResult<ExportPortfolioReportResponse>> ExportPortfolio([FromBody] ExportPortfolioReportRequest request, CancellationToken ct)
        => Ok(await Mediator.Send(new ExportPortfolioReportQuery(GetOrganizationId(), request.Format, request.AsOfDate, request.FilterStatus ?? []), ct));

    [HttpPost("risk/provisioning-charges")]
    [Authorize(Roles = "RiskAnalyst,Admin")]
    public async Task<ActionResult<TriggerProvisioningChargeResponse>> TriggerProvisioning([FromBody] TriggerProvisioningChargeRequest request, CancellationToken ct)
        => Ok(await Mediator.Send(new TriggerProvisioningChargeCommand(GetOrganizationId(), request.Period, request.ProvisioningModel), ct));
}

public sealed record ExportPortfolioReportRequest(string Format, DateOnly AsOfDate, List<string>? FilterStatus);
public sealed record TriggerProvisioningChargeRequest(string Period, string ProvisioningModel);
