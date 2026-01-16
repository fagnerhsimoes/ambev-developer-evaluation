using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<UpdateSaleHandler> logger) : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {

        var sale = await saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        // Map updates
        mapper.Map(command, sale);
        
        // Recalculate totals
        sale.CalculateTotalAmount();
        
        // Handle cancellation
        if (command.IsCancelled)
        {
            sale.Cancel();
            logger.LogInformation("Event Published: SaleCancelledEvent for Sale ID {SaleId}", sale.Id);
        }

        sale.UpdatedAt = DateTime.UtcNow;

        await saleRepository.UpdateAsync(sale, cancellationToken);
        
        logger.LogInformation("Event Published: SaleModifiedEvent for Sale ID {SaleId}", sale.Id);

        var result = mapper.Map<UpdateSaleResult>(sale);
        return result;
    }
}
