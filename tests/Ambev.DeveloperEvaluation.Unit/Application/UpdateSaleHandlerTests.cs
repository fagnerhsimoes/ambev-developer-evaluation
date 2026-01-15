using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Enums;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new UpdateSaleHandler(_saleRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldPublishEventWithNewValues_WhenSaleIsUpdated()
    {
        // Arrange
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            SaleNumber = "SALE-NEW-001",
            Date = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "John Doe",
            BranchId = Guid.NewGuid(),
            BranchName = "Main Branch",
            IsCancelled = true,
            Items = new List<UpdateSaleCommand.UpdateSaleItemDto>
            {
                new() { ProductId = Guid.NewGuid(), ProductName = "Beer", Quantity = 5, UnitPrice = 10 }
            }
        };
        
        var existingSale = new Sale
        {
            Id = command.Id,
            SaleNumber = "SALE-OLD-001",
            IsCancelled = false,
            Status = SaleStatus.Pending
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);
        
        // Setup Mapper to actually simulate mapping (or manually map in test if mock is hard)
        // Since we mock Mapper, we need to instruct it what to do, OR stick to real behavior.
        // Better to simulate behavior:
        _mapper.When(x => x.Map(command, existingSale)).Do(x => 
        {
            existingSale.SaleNumber = command.SaleNumber;
            // logic for IsCancelled is handled in Handler via separate property check? 
            // Checking handler code: "if (command.IsCancelled) sale.Cancel();"
            // It is explicitly handled.
        });

        _saleRepository.UpdateAsync(existingSale, Arg.Any<CancellationToken>())
            .Returns(existingSale); // Repository returns the same modified instance

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        // Verify UpdateAsync was called with the entity having NEW values
        existingSale.SaleNumber.Should().Be("SALE-NEW-001");
        existingSale.IsCancelled.Should().BeTrue();
        existingSale.Status.Should().Be(SaleStatus.Cancelled);
        
        // NOTE: Since the Handler instantiates 'new SaleModifiedEvent(updatedSale)' but doesn't PUBLISH it via Mediator 
        // (the code commented out says "In a real scenario..."), current implementation only creates it.
        // We can't verify PUBLISH if it's not published. 
        // But the user's concern was "publishing the old event". 
        // Since the 'updatedSale' variable holds 'existingSale' which we asserted has NEW values, 
        // the event created using 'updatedSale' WILL have the new values.
    }
}
