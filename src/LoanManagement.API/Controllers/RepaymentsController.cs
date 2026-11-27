using LoanManagement.Application.Features.Repayments.Commands.MakeRepayment;
using LoanManagement.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagement.API.Controllers;

public sealed class RepaymentsController : BaseApiController
{
    [HttpPost("loans/{id}/repayments")]
    [Authorize(Roles = "Customer,Collector,BackOffice,Admin")]
    public async Task<ActionResult<MakeRepaymentResponse>> MakeRepayment(string id, [FromBody] MakeRepaymentRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, true, out var method))
            return BadRequest(new { error = "Invalid payment method" });

        var result = await Mediator.Send(new MakeRepaymentCommand(GetOrganizationId(), id, request.PaymentAmount, method, request.PaymentReference, request.PaidAt), ct);
        return Ok(result);
    }
}

public sealed record MakeRepaymentRequest(decimal PaymentAmount, string PaymentMethod, string PaymentReference, DateTimeOffset PaidAt);
