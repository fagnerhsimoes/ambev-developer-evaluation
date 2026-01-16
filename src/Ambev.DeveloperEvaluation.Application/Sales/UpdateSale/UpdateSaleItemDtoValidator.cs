using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleItemDtoValidator : AbstractValidator<UpdateSaleCommand.UpdateSaleItemDto>
{
    public UpdateSaleItemDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.ProductName).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0).LessThanOrEqualTo(20).WithMessage("Quantity must be between 1 and 20.");
        RuleFor(x => x.UnitPrice).GreaterThan(0);
    }
}