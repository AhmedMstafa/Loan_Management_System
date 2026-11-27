using LoanManagement.Application.Common.Exceptions;
using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Features.Products.Commands.ManageProductRates;

public sealed class ManageProductRatesCommandHandler : IRequestHandler<ManageProductRatesCommand, ManageProductRatesResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;
    public ManageProductRatesCommandHandler(IApplicationDbContext db, IDateTimeProvider clock) { _db = db; _clock = clock; }

    public async Task<ManageProductRatesResponse> Handle(ManageProductRatesCommand request, CancellationToken cancellationToken)
    {
        var product = await _db.LoanProducts.FirstOrDefaultAsync(x => x.Id == request.ProductId && x.OrganizationId == request.OrganizationId, cancellationToken)
            ?? throw new NotFoundException(nameof(LoanProduct), request.ProductId);

        product.UpdateRates(request.BaseInterestRate, request.MinLoanAmount, request.MaxLoanAmount, request.OriginationFeePercentage, request.LateFeeFlatRate);
        var now = _clock.UtcNow;
        product.MarkUpdated(now);
        await _db.SaveChangesAsync(cancellationToken);
        return new ManageProductRatesResponse(product.Id, "Updated", request.EffectiveDate, now);
    }
}
