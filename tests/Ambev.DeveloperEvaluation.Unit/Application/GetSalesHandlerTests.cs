using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class GetSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSalesHandler _handler;

    public GetSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSalesHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid command When handled Then returns paginated list")]
    public async Task Handle_ValidCommand_ReturnsPaginatedList()
    {
        // Arrange
        var command = new GetSalesCommand
        {
            Page = 1,
            Size = 10,
            Order = "SaleNumber"
        };

        var sales = new List<Sale>
        {
            SaleTestData.GenerateValidSale(),
            SaleTestData.GenerateValidSale()
        };
        
        var totalCount = 2;

        _saleRepository.GetAllAsync(command.Page, command.Size, command.Order, Arg.Any<CancellationToken>())
            .Returns((sales, totalCount));

        var resultDtos = sales.Select(s => new GetSaleResult { Id = s.Id, SaleNumber = s.SaleNumber }).ToList();
        _mapper.Map<List<GetSaleResult>>(sales).Returns(resultDtos);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(10);
        
        await _saleRepository.Received(1).GetAllAsync(command.Page, command.Size, command.Order, Arg.Any<CancellationToken>());
    }
}
