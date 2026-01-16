using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly DeleteSaleHandler _handler;

    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        var logger = Substitute.For<ILogger<DeleteSaleHandler>>();
        _handler = new DeleteSaleHandler(_saleRepository, logger);
    }

    [Fact(DisplayName = "Cancel Sale should update status to Cancelled and IsCancelled to true")]
    public async Task Handle_ValidId_ShouldCancelSale()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = new Sale { Id = saleId, Status = SaleStatus.Pending, IsCancelled = false };
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);

        var command = new DeleteSaleCommand(saleId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        sale.IsCancelled.Should().BeTrue();
        sale.Status.Should().Be(SaleStatus.Cancelled);
        await _saleRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Cancel non-existent sale should throw KeyNotFoundException")]
    public async Task Handle_InvalidId_ShouldThrowException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        var command = new DeleteSaleCommand(saleId);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
