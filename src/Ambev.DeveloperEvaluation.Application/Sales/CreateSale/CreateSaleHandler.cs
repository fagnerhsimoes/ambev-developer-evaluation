using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler(ISaleRepository saleRepository, IUserRepository userRepository, IMapper mapper, ILogger<CreateSaleHandler> logger)
    : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {

        // Customer Validation (External Identity)
        var customer = await userRepository.GetByIdAsync(command.CustomerId, cancellationToken);
        if (customer == null)
            throw new ValidationException([
                new FluentValidation.Results.ValidationFailure("CustomerId", $"Customer with ID {command.CustomerId} not found")
            ]);


        var sale = mapper.Map<Sale>(command);
        
        // Ensure verified customer name is used and set branch (External Identity Pattern)
        sale.SetCustomer(command.CustomerId, customer.Username);
        sale.SetBranch(command.BranchId, command.BranchName);
        
        // Calculate total for the sale (internally calculates items)
        sale.CalculateTotalAmount();

        var createdSale = await saleRepository.CreateAsync(sale, cancellationToken);
        
        // Simulate Event Publishing
        logger.LogInformation("Event Published: SaleCreatedEvent for Sale ID {SaleId}", createdSale.Id);

        var result = mapper.Map<CreateSaleResult>(createdSale);
        return result;
    }
}
