using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper) : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {

        var sale = await saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        // Map updates
        mapper.Map(command, sale);
        
        // Recalculate totals
        foreach (var item in sale.SaleItems)
        {
            item.CalculateTotal();
        }
        sale.CalculateTotalAmount();
        
        // Handle cancellation
        if (command.IsCancelled)
        {
            sale.Cancel();
        }

        sale.UpdatedAt = DateTime.UtcNow;

        await saleRepository.UpdateAsync(sale, cancellationToken);
        
        var result = mapper.Map<UpdateSaleResult>(sale);
        return result;
    }
}
