using Ambev.DeveloperEvaluation.Domain.Entities;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleItemTests
{
    [Fact(DisplayName = "Quantity 3 should have 0% discount")]
    public void Given_QuantityIs3_When_Calculated_Then_DiscountShouldBeZero()
    {
        var item = new SaleItem { Quantity = 3, UnitPrice = 100 };
        item.CalculateTotal();
        Assert.Equal(0m, item.Discount);
        Assert.Equal(300m, item.TotalAmount);
    }

    [Fact(DisplayName = "Quantity 5 should have 10% discount")]
    public void Given_QuantityIs5_When_Calculated_Then_DiscountShouldBe10Percent()
    {
        var item = new SaleItem { Quantity = 5, UnitPrice = 100 };
        item.CalculateTotal();
        Assert.Equal(10m, item.Discount);
        Assert.Equal(450m, item.TotalAmount); // 5 * 100 * 0.9 = 450
    }

    [Fact(DisplayName = "Quantity 15 should have 20% discount")]
    public void Given_QuantityIs15_When_Calculated_Then_DiscountShouldBe20Percent()
    {
        var item = new SaleItem { Quantity = 15, UnitPrice = 100 };
        item.CalculateTotal();
        Assert.Equal(20m, item.Discount);
        Assert.Equal(1200m, item.TotalAmount); // 15 * 100 * 0.8 = 1200
    }

    [Fact(DisplayName = "Quantity 21 should throw exception")]
    public void Given_QuantityIs21_When_Calculated_Then_ShouldThrowException()
    {
        var item = new SaleItem { Quantity = 21, UnitPrice = 100m };
        Assert.Throws<InvalidOperationException>(() => item.CalculateTotal());
    }

    [Fact(DisplayName = "Quantity less than 1 should throw exception")]
    public void Given_QuantityIsZero_When_Calculated_Then_ShouldThrowException()
    {
        var item = new SaleItem { Quantity = 0, UnitPrice = 100m };
        Assert.Throws<InvalidOperationException>(() => item.CalculateTotal());
    }

    [Fact(DisplayName = "Cancel should set IsCancelled to true and TotalAmount to zero")]
    public void Given_Item_When_Cancelled_Then_ShouldBeCancelled()
    {
        // Arrange
        var item = new SaleItem { Quantity = 5, UnitPrice = 100 };
        item.CalculateTotal();

        // Act
        item.Cancel();

        // Assert
        Assert.True(item.IsCancelled);
        Assert.Equal(0, item.TotalAmount);
    }
}