using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public class DeleteSaleHandler(ISaleRepository saleRepository, ILogger<DeleteSaleHandler> logger) : IRequestHandler<DeleteSaleCommand, DeleteSaleResponse>
{
    public async Task<DeleteSaleResponse> Handle(DeleteSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        // Cancel instead of physical delete
        sale.Cancel();
        await saleRepository.UpdateAsync(sale, cancellationToken);
        
        logger.LogInformation("Event Published: SaleCancelledEvent for Sale ID {SaleId}", sale.Id);

        return new DeleteSaleResponse { Success = true };
    }
}
