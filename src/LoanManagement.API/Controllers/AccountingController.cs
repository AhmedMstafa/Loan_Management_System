using LoanManagement.Application.Features.Accounting.Commands.PostGlJournalEntry;
using LoanManagement.Application.Features.Accounting.Commands.UploadBankReconciliation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagement.API.Controllers;

public sealed class AccountingController : BaseApiController
{
    [HttpPost("accounting/bank-reconciliations")]
    [Authorize(Roles = "Ops,Accountant,Admin")]
    public async Task<ActionResult<UploadBankReconciliationResponse>> Reconcile([FromBody] UploadBankReconciliationCommand command, CancellationToken ct)
        => Ok(await Mediator.Send(command with { OrganizationId = GetOrganizationId() }, ct));

    [HttpPost("accounting/gl-journal-entries")]
    [Authorize(Roles = "Accountant,Admin")]
    public async Task<ActionResult<PostGlJournalEntryResponse>> PostJournal([FromBody] PostGlJournalEntryCommand command, CancellationToken ct)
    {
        var result = await Mediator.Send(command with { OrganizationId = GetOrganizationId() }, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
}
