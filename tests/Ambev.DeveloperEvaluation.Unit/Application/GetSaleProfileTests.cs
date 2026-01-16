using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class GetSaleProfileTests
{
    private readonly IMapper _mapper;

    public GetSaleProfileTests()
    {
        var configuration = new MapperConfiguration(cfg =>
            cfg.AddProfile<GetSaleResultProfile>());

        _mapper = configuration.CreateMapper();
    }

    [Fact(DisplayName = "Valid Sale maps to GetSaleResult correctly")]
    public void Given_Sale_When_Mapped_Then_ConvertsToGetSaleResult()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        var result = _mapper.Map<GetSaleResult>(sale);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);
        result.SaleNumber.Should().Be(sale.SaleNumber);
        result.SaleDate.Should().Be(sale.SaleDate);
        result.CustomerId.Should().Be(sale.CustomerId);
        result.CustomerName.Should().Be(sale.CustomerName);
        result.TotalAmount.Should().Be(sale.TotalAmount);
        result.IsCancelled.Should().Be(sale.IsCancelled);
        result.SaleItems.Should().HaveCount(sale.SaleItems.Count);

        // Check Item mapping
        var firstItem = sale.SaleItems.First();
        var firstResultItem = result.SaleItems.First();
        firstResultItem.ProductName.Should().Be(firstItem.ProductName);
        firstResultItem.Quantity.Should().Be(firstItem.Quantity);
        firstResultItem.TotalAmount.Should().Be(firstItem.TotalAmount);
    }
}
