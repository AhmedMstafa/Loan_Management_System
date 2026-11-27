using FluentValidation;

namespace LoanManagement.Application.Features.Loans.Commands.CreateLoanApplication;

public sealed class CreateLoanCommandValidator : AbstractValidator<CreateLoanCommand>
{
    public CreateLoanCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.BorrowerId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.RequestedAmount).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
        RuleFor(x => x.TenorMonths).GreaterThan(0);
        RuleFor(x => x.AnnualInterestRate).GreaterThanOrEqualTo(0);
    }
}
