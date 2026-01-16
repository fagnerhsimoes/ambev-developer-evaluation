using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CreateSaleProfileTests
{
    private readonly IMapper _mapper;

    public CreateSaleProfileTests()
    {
        var configuration = new MapperConfiguration(cfg =>
            cfg.AddProfile<CreateSaleProfile>());

        _mapper = configuration.CreateMapper();
    }

    [Fact(DisplayName = "Valid CreateSaleCommand maps to Sale correctly")]
    public void Given_Command_When_Mapped_Then_ConvertsToSale()
    {
        // Arrange
        var command = CreateSaleHandlerTestData.GenerateValidCommand();

        // Act
        var sale = _mapper.Map<Sale>(command);

        // Assert
        sale.Should().NotBeNull();
        sale.SaleNumber.Should().Be(command.SaleNumber);
        sale.SaleDate.Should().Be(command.Date);
        // CustomerId IS NOT MAPPED in profile? Checking implicit mapping.
        // CreateSaleCommand has CustomerId. Sale has CustomerId. AutoMapper maps by name by default.
        sale.CustomerId.Should().Be(command.CustomerId);

        // CustomerName is NOT in profile explicitly and likely handled by logic, but if properties match, it maps.
        // Command has CustomerName. Sale has CustomerName. AutoMapper SHOULD map it unless ignored.
        // Wait, handler calls SetCustomer overriding it. But Mapper should map it if names match.
        // Let's verify.
        sale.CustomerName.Should().Be(command.CustomerName);

        sale.SaleItems.Should().HaveCount(command.Items.Count);
    }
}
