using LoanManagement.Application.Features.Loans.Commands.UploadKycDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagement.API.Controllers;

public sealed class BorrowersController : BaseApiController
{
    [HttpPost("borrowers/{id}/kyc-documents")]
    [Authorize(Roles = "Compliance,LoanOfficer,Admin")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<UploadKycDocumentResponse>> UploadKyc(string id, [FromForm] UploadKycRequest request, CancellationToken ct)
    {
        await using var ms = new MemoryStream();
        await request.File.CopyToAsync(ms, ct);
        var result = await Mediator.Send(new UploadKycDocumentCommand(GetOrganizationId(), id, request.DocumentType, request.File.FileName, ms.ToArray()), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
}

public sealed class UploadKycRequest
{
    public string DocumentType { get; set; } = string.Empty;
    public IFormFile File { get; set; } = default!;
}
