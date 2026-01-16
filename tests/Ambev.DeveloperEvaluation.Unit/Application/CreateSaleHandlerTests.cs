using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Enums;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateSaleHandler(_saleRepository, _userRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid command When handled Then creates sale successfully")]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            SaleNumber = "SALE-001",
            Date = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Customer Name",
            BranchName = "Branch A",
            BranchId = Guid.NewGuid(),
            Items = [new() { ProductId = Guid.NewGuid(), ProductName = "Product A", Quantity = 5, UnitPrice = 100 }]
        };

        var customer = new User { Id = command.CustomerId, Username = "Customer Name" };
        _userRepository.GetByIdAsync(command.CustomerId, Arg.Any<CancellationToken>()).Returns(customer);

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            CustomerId = command.CustomerId,
            SaleItems = []
        };
        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(sale, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(new CreateSaleResult { Id = sale.Id });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        await _saleRepository.Received(1).CreateAsync(sale, Arg.Any<CancellationToken>());
        sale.CustomerName.Should().Be("Customer Name");
        sale.Status.Should().Be(SaleStatus.Pending);
    }

    [Fact(DisplayName = "Given invalid customer When handled Then throws ValidationException")]
    public async Task Handle_InvalidCustomer_ThrowsException()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            SaleNumber = "SALE-001",
            Date = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "John Doe",
            BranchName = "Branch A",
            BranchId = Guid.NewGuid(),
             Items = [new() { ProductId = Guid.NewGuid(), ProductName = "Beer", Quantity = 5, UnitPrice = 100 }]
        };

        _userRepository.GetByIdAsync(command.CustomerId, Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage($"*Customer with ID {command.CustomerId} not found*");
    }
}
