using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleValidator : AbstractValidator<Sale>
{
    public SaleValidator()
    {
        RuleFor(x => x.SaleNumber)
            .NotEmpty()
            .WithMessage("Sale Number is required.");

        RuleFor(x => x.SaleDate)
            .NotEmpty()
            .WithMessage("SaleDate is required.");

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer Id is required.");
        
        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("Customer Name is required.");
        
        RuleFor(x => x.BranchId)
            .NotEmpty()
            .WithMessage("Branch Id is required.");
            
        RuleFor(x => x.BranchName)
            .NotEmpty()
            .WithMessage("Branch Name is required.");

        RuleFor(x => x.SaleItems)
            .NotEmpty()
            .WithMessage("Sale must contain at least one item.");

        RuleForEach(x => x.SaleItems).SetValidator(new SaleItemValidator());
    }
}
