using LoanManagement.Application.Features.Loans.Commands.ApplyTopUp;
using LoanManagement.Application.Features.Loans.Commands.ApproveLoan;
using LoanManagement.Application.Features.Loans.Commands.BulkImportLoans;
using LoanManagement.Application.Features.Loans.Commands.CreateLoanApplication;
using LoanManagement.Application.Features.Loans.Commands.DisburseLoan;
using LoanManagement.Application.Features.Loans.Commands.ResolveCustomerDispute;
using LoanManagement.Application.Features.Loans.Commands.RunCreditCheck;
using LoanManagement.Application.Features.Loans.Commands.UploadKycDocument;
using LoanManagement.Application.Features.Loans.Queries.GetDelinquentLoans;
using LoanManagement.Application.Features.Loans.Queries.GetLoanStatement;
using LoanManagement.Application.Features.Loans.Queries.GetPaymentSchedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagement.API.Controllers;

public sealed class LoansController : BaseApiController
{
    [HttpPost("loans")]
    [Authorize(Roles = "LoanOfficer,Admin")]
    public async Task<ActionResult<CreateLoanResponse>> Create([FromBody] CreateLoanRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(new CreateLoanCommand(GetOrganizationId(), request.BorrowerId, request.ProductId, request.RequestedAmount, request.Currency, request.TenorMonths, request.AnnualInterestRate, request.Purpose, request.CollateralDescription), ct);
        return CreatedAtAction(nameof(GetPaymentSchedule), new { id = result.Id }, result);
    }

    [HttpPost("loans/{id}/credit-check")]
    [Authorize(Roles = "Underwriter,Admin")]
    public async Task<ActionResult<RunCreditCheckResponse>> RunCreditCheck(string id, [FromBody] RunCreditCheckRequest request, CancellationToken ct)
        => Ok(await Mediator.Send(new RunCreditCheckCommand(GetOrganizationId(), id, request.Provider, request.ForceRefresh), ct));

    [HttpPost("loans/{id}/approve")]
    [Authorize(Roles = "Underwriter,Admin")]
    public async Task<ActionResult<ApproveLoanResponse>> Approve(string id, [FromBody] ApproveLoanRequest request, CancellationToken ct)
        => Ok(await Mediator.Send(new ApproveLoanCommand(GetOrganizationId(), id, request.ApprovedAmount, request.ApprovedInterestRate, request.TenorMonths, request.Notes ?? "", request.ScheduledDisbursementDate, User.FindFirst("sub")?.Value ?? User.Identity?.Name ?? "system"), ct));

    [HttpPost("loans/{id}/disburse")]
    [Authorize(Roles = "BackOffice,Admin")]
    public async Task<ActionResult<DisburseLoanResponse>> Disburse(string id, [FromBody] DisburseLoanRequest request, CancellationToken ct)
        => Ok(await Mediator.Send(new DisburseLoanCommand(GetOrganizationId(), id, request.DisbursedAmount, request.DisbursementMethod, request.DestinationAccount.BankCode, request.DestinationAccount.AccountNumber, request.DestinationAccount.AccountHolderName, request.FirstRepaymentDueDate), ct));

    [HttpGet("loans/{id}/payment-schedule")]
    [Authorize(Roles = "Customer,LoanOfficer,BackOffice,Admin")]
    public async Task<ActionResult<GetPaymentScheduleResponse>> GetPaymentSchedule(string id, CancellationToken ct)
        => Ok(await Mediator.Send(new GetPaymentScheduleQuery(GetOrganizationId(), id), ct));

    [HttpGet("loans/delinquent")]
    [Authorize(Roles = "Collector,RiskAnalyst,Admin")]
    public async Task<IActionResult> GetDelinquent([FromQuery] int minDaysOverdue = 30, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => Ok(await Mediator.Send(new GetDelinquentLoansQuery(GetOrganizationId(), minDaysOverdue, pageNumber, pageSize), ct));

    [HttpGet("loans/{id}/statement")]
    [Authorize(Roles = "Customer,LoanOfficer,Admin")]
    public async Task<ActionResult<GetLoanStatementResponse>> GetStatement(string id, [FromQuery] DateOnly from, [FromQuery] DateOnly to, [FromQuery] string format = "PDF", CancellationToken ct = default)
        => Ok(await Mediator.Send(new GetLoanStatementQuery(GetOrganizationId(), id, from, to, format), ct));

    [HttpPost("loans/bulk-import")]
    [Authorize(Roles = "Admin")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<BulkImportLoansResponse>> BulkImport([FromForm] BulkImportLoansRequest request, CancellationToken ct)
    {
        await using var ms = new MemoryStream();
        await request.File.CopyToAsync(ms, ct);
        var result = await Mediator.Send(new BulkImportLoansCommand(GetOrganizationId(), request.File.FileName, ms.ToArray()), ct);
        return Accepted(result);
    }

    [HttpPost("loans/{id}/top-up")]
    [Authorize(Roles = "Customer,LoanOfficer,Admin")]
    public async Task<ActionResult<ApplyTopUpResponse>> TopUp(string id, [FromBody] ApplyTopUpRequest request, CancellationToken ct)
        => Ok(await Mediator.Send(new ApplyTopUpCommand(GetOrganizationId(), id, request.AdditionalAmount, request.TenorExtensionMonths, request.Reason), ct));

    [HttpPost("loans/{id}/disputes/resolve")]
    [Authorize(Roles = "Support,Admin")]
    public async Task<ActionResult<ResolveCustomerDisputeResponse>> ResolveDispute(string id, [FromBody] ResolveDisputeRequest request, CancellationToken ct)
        => Ok(await Mediator.Send(new ResolveCustomerDisputeCommand(GetOrganizationId(), id, request.DisputeId, request.ResolutionType, request.AdjustmentAmount, request.ResolutionNotes, User.FindFirst("sub")?.Value ?? User.Identity?.Name ?? "system"), ct));
}

public sealed record CreateLoanRequest(string OrganizationId, string BorrowerId, string ProductId, decimal RequestedAmount, string Currency, int TenorMonths, decimal AnnualInterestRate, string? Purpose, string? CollateralDescription);
public sealed record RunCreditCheckRequest(string Provider, bool ForceRefresh);
public sealed record ApproveLoanRequest(decimal ApprovedAmount, decimal ApprovedInterestRate, int TenorMonths, string? Notes, DateOnly ScheduledDisbursementDate);
public sealed record DisburseLoanRequest(decimal DisbursedAmount, string DisbursementMethod, DestinationAccountDto DestinationAccount, DateOnly FirstRepaymentDueDate);
public sealed record DestinationAccountDto(string BankCode, string AccountNumber, string AccountHolderName);
public sealed record ApplyTopUpRequest(decimal AdditionalAmount, int TenorExtensionMonths, string Reason);
public sealed record ResolveDisputeRequest(string DisputeId, string ResolutionType, decimal AdjustmentAmount, string ResolutionNotes);
public sealed class BulkImportLoansRequest
{
    public IFormFile File { get; set; } = default!;
}
