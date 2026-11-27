using LoanManagement.Application.Common.Exceptions;
using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Features.Loans.Commands.CreateLoanApplication;

public sealed class CreateLoanCommandHandler : IRequestHandler<CreateLoanCommand, CreateLoanResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;

    public CreateLoanCommandHandler(IApplicationDbContext db, IDateTimeProvider clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<CreateLoanResponse> Handle(CreateLoanCommand request, CancellationToken cancellationToken)
    {
        var borrower = await _db.Borrowers.FirstOrDefaultAsync(x => x.Id == request.BorrowerId && x.OrganizationId == request.OrganizationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Borrower), request.BorrowerId);
        var product = await _db.LoanProducts.FirstOrDefaultAsync(x => x.Id == request.ProductId && x.OrganizationId == request.OrganizationId, cancellationToken)
            ?? throw new NotFoundException(nameof(LoanProduct), request.ProductId);

        if (request.RequestedAmount < product.MinAmount || request.RequestedAmount > product.MaxAmount)
            throw new FluentValidation.ValidationException("Requested amount is outside product limits.");

        var now = _clock.UtcNow;
        var loan = new Loan(
            id: $"loa_{Guid.NewGuid():N}"[..16],
            organizationId: request.OrganizationId,
            borrowerId: borrower.Id,
            productId: product.Id,
            requestedAmount: request.RequestedAmount,
            currency: request.Currency,
            tenorMonths: request.TenorMonths,
            annualInterestRate: request.AnnualInterestRate,
            purpose: request.Purpose,
            collateralDescription: request.CollateralDescription);
        loan.MarkCreated(now, borrower.Id);

        _db.Loans.Add(loan);
        await _db.SaveChangesAsync(cancellationToken);

        return new CreateLoanResponse(loan.Id, loan.Status.ToString(), loan.OrganizationId, loan.BorrowerId, loan.RequestedAmount, loan.TenorMonths, loan.CreatedAt);
    }
}
