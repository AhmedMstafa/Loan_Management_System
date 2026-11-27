using LoanManagement.Application.Features.Products.Commands.ManageProductRates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagement.API.Controllers;

public sealed class ProductsController : BaseApiController
{
    [HttpPut("products/{id}/rates")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ManageProductRatesResponse>> UpdateRates(string id, [FromBody] ManageProductRatesRequest request, CancellationToken ct)
        => Ok(await Mediator.Send(new ManageProductRatesCommand(GetOrganizationId(), id, request.BaseInterestRate, request.MinLoanAmount, request.MaxLoanAmount, request.OriginationFeePercentage, request.LateFeeFlatRate, request.EffectiveDate), ct));
}

public sealed record ManageProductRatesRequest(decimal BaseInterestRate, decimal MinLoanAmount, decimal MaxLoanAmount, decimal OriginationFeePercentage, decimal LateFeeFlatRate, DateOnly EffectiveDate);
