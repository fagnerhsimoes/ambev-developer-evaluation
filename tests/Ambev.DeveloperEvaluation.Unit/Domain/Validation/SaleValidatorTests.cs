using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Validation;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Validation;

public class SaleValidatorTests
{
    private readonly SaleValidator _validator = new();

    [Fact(DisplayName = "Valid Sale should pass validation")]
    public void Given_ValidSale_When_Validated_Then_ShouldNotHaveErrors()
    {
        var sale = new Sale
        {
            SaleNumber = "SALE-001",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "John Doe",
            BranchName = "Branch 1",
            BranchId = Guid.NewGuid(),
            Status = SaleStatus.Pending,
            SaleItems =
            [
                new SaleItem { Quantity = 5, UnitPrice = 10, ProductId = Guid.NewGuid(), ProductName = "Product A" }
            ]
        };

        var result = _validator.TestValidate(sale);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Empty SaleNumber should fail validation")]
    public void Given_EmptySaleNumber_When_Validated_Then_ShouldHaveError()
    {
        var sale = new Sale { SaleNumber = "" };
        var result = _validator.TestValidate(sale);
        result.ShouldHaveValidationErrorFor(x => x.SaleNumber);
    }

    [Fact(DisplayName = "Empty CustomerId should fail validation")]
    public void Given_EmptyCustomerId_When_Validated_Then_ShouldHaveError()
    {
        var sale = new Sale { CustomerId = Guid.Empty };
        var result = _validator.TestValidate(sale);
        result.ShouldHaveValidationErrorFor(x => x.CustomerId);
    }

    [Fact(DisplayName = "Empty Items should fail validation")]
    public void Given_EmptyItems_When_Validated_Then_ShouldHaveError()
    {
        var sale = new Sale { SaleItems = [] };
        var result = _validator.TestValidate(sale);
        result.ShouldHaveValidationErrorFor(x => x.SaleItems);
    }
}
