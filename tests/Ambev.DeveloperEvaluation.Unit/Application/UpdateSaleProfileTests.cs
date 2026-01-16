using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class UpdateSaleProfileTests
{
    private readonly IMapper _mapper;

    public UpdateSaleProfileTests()
    {
        var configuration = new MapperConfiguration(cfg =>
            cfg.AddProfile<UpdateSaleProfile>());

        _mapper = configuration.CreateMapper();
    }

    [Fact(DisplayName = "Valid UpdateSaleCommand maps to Sale correctly")]
    public void Given_Command_When_Mapped_Then_ConvertsToSale()
    {
        // Arrange
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();

        // Act
        var sale = _mapper.Map<Sale>(command);

        // Assert
        sale.Should().NotBeNull();
        sale.Id.Should().Be(command.Id);
        sale.SaleNumber.Should().Be(command.SaleNumber);
        sale.SaleDate.Should().Be(command.Date);
        sale.CustomerId.Should().Be(command.CustomerId);
        sale.SaleItems.Should().HaveCount(command.Items.Count);
    }
}