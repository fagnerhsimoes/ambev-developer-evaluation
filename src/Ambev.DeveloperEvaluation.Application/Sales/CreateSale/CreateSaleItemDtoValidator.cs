using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleItemDtoValidator : AbstractValidator<CreateSaleCommand.CreateSaleItemDto>
{
    public CreateSaleItemDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.ProductName).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0).LessThanOrEqualTo(20).WithMessage("Quantity must be between 1 and 20.");
        RuleFor(x => x.UnitPrice).GreaterThan(0);
    }
}