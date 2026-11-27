using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagement.API.Controllers;

[ApiController]
[Route("api/v1")]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected string GetOrganizationId()
        => User.FindFirst("org_id")?.Value
           ?? User.FindFirst("organizationId")?.Value
           ?? Request.Headers["X-Organization-Id"].FirstOrDefault()
           ?? "org_default";
}
