using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

public class GetSalesCommandValidator : AbstractValidator<GetSalesCommand>
{
    public GetSalesCommandValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.Size)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100 items.");
    }
}
