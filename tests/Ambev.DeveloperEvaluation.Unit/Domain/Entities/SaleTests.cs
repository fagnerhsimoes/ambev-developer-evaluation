using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    [Fact]
    public void CalculateTotalAmount_ShouldSumAllActiveItems()
    {
        // Arrange
        var sale = new Sale();
        var item1 = new SaleItem { ProductId = Guid.NewGuid(), ProductName = "Item 1", Quantity = 2, UnitPrice = 10 }; // Total = 20 (0% disc)
        
        var item2 = new SaleItem { ProductId = Guid.NewGuid(), ProductName = "Item 2", Quantity = 5, UnitPrice = 10 }; // 50 * 0.9 = 45 (10% disc)
        
        var item3 = new SaleItem { ProductId = Guid.NewGuid(), ProductName = "Item 3", Quantity = 1, UnitPrice = 100 }; 
        item3.Cancel(); // Should be ignored

        sale.AddItem(item1);
        sale.AddItem(item2);
        sale.AddItem(item3);

        // Act
        sale.CalculateTotalAmount();

        // Assert
        sale.TotalAmount.Should().Be(65);
    }
    
    [Fact]
    public void CalculateTotalAmount_WhenSameProductExceeds20_ShouldThrowException()
    {
        var sale = new Sale();
        var prodId = Guid.NewGuid();
        // 15 + 10 = 25 (> 20)
        sale.AddItem(new SaleItem { ProductId = prodId, ProductName = "Beer", Quantity = 15, UnitPrice = 10 });
        
        // The second add will trigger CalculateTotalAmount which checks the rule
        Assert.Throws<Ambev.DeveloperEvaluation.Domain.Exceptions.DomainException>(() => 
            sale.AddItem(new SaleItem { ProductId = prodId, ProductName = "Beer", Quantity = 10, UnitPrice = 10 })
        );
    }

    [Fact]
    public void Cancel_ShouldCancelSaleAndAllItems()
    {
        // Arrange
        var sale = new Sale();
        var item1 = new SaleItem { ProductId = Guid.NewGuid(), ProductName = "Item 1", Quantity = 2, UnitPrice = 10 };
        sale.AddItem(item1);

        // Act
        sale.Cancel();

        // Assert
        sale.IsCancelled.Should().BeTrue();
        sale.Status.Should().Be(SaleStatus.Cancelled);
        sale.CancelledAt.Should().NotBeNull();
        sale.SaleItems.All(i => i.IsCancelled).Should().BeTrue();
    }

    [Fact]
    public void SetCustomer_ShouldUpdateCustomerAndUpdatedAt()
    {
        // Arrange
        var sale = new Sale();
        var customerId = Guid.NewGuid();
        var customerName = "Test Customer";

        // Act
        sale.SetCustomer(customerId, customerName);

        // Assert
        sale.CustomerId.Should().Be(customerId);
        sale.CustomerName.Should().Be(customerName);
        sale.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void SetBranch_ShouldUpdateBranchAndUpdatedAt()
    {
        // Arrange
        var sale = new Sale();
        var branchId = Guid.NewGuid();
        var branchName = "Test Branch";

        // Act
        sale.SetBranch(branchId, branchName);

        // Assert
        sale.BranchId.Should().Be(branchId);
        sale.BranchName.Should().Be(branchName);
        sale.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Validate_WhenNoItems_ShouldReturnInvalid()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-001",
            CustomerId = Guid.NewGuid(),
            CustomerName = "Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Branch"
        };

        // Act
        var result = sale.Validate();

        // Assert
        result.IsValid.Should().BeFalse();
        // The exact error message depends on FluentValidation's SaleValidator
        result.Errors.Should().Contain(e => e.Error == "NotEmptyValidator");
    }

    [Fact]
    public void Validate_WhenValidSale_ShouldReturnValid()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-001",
            CustomerId = Guid.NewGuid(),
            CustomerName = "Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Branch"
        };
        var item = new SaleItem { ProductId = Guid.NewGuid(), ProductName = "Product", Quantity = 10, UnitPrice = 10 };
        sale.AddItem(item);

        // Act
        var result = sale.Validate();

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
