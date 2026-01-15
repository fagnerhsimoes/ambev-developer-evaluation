using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Validation;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Validation;

public class SaleItemValidatorTests
{
    private readonly SaleItemValidator _validator;

    public SaleItemValidatorTests()
    {
        _validator = new SaleItemValidator();
    }

    [Fact(DisplayName = "Quantity 0 should fail validation")]
    public void Given_QuantityZero_When_Validated_Then_ShouldHaveError()
    {
        var item = new SaleItem { Quantity = 0 };
        var result = _validator.TestValidate(item);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact(DisplayName = "Quantity greater than 20 should fail validation")]
    public void Given_QuantityGreaterThan20_When_Validated_Then_ShouldHaveError()
    {
        var item = new SaleItem { Quantity = 21 };
        var result = _validator.TestValidate(item);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact(DisplayName = "Quantity 4 should not have error")]
    public void Given_Quantity4_When_Validated_Then_ShouldNotHaveError()
    {
        var item = new SaleItem { Quantity = 4, UnitPrice = 10, ProductId = Guid.NewGuid(), ProductName = "Product A" };
        var result = _validator.TestValidate(item);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
