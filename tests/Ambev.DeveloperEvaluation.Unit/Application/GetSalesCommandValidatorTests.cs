using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class GetSalesCommandValidatorTests
{
    private readonly GetSalesCommandValidator _validator = new();

    [Fact(DisplayName = "Valid command should pass validation")]
    public void Given_ValidCommand_When_Validated_Then_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new GetSalesCommand
        {
            Page = 1,
            Size = 10,
            Order = "SaleNumber"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory(DisplayName = "Invalid Page should fail validation")]
    [InlineData(0)]
    [InlineData(-1)]
    public void Given_InvalidPage_When_Validated_Then_ShouldHaveError(int page)
    {
        // Arrange
        var command = new GetSalesCommand
        {
            Page = page,
            Size = 10
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Theory(DisplayName = "Invalid Size should fail validation")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)] // Max is 100
    public void Given_InvalidSize_When_Validated_Then_ShouldHaveError(int size)
    {
        // Arrange
        var command = new GetSalesCommand
        {
            Page = 1,
            Size = size
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Size);
    }
}
