using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public class DeleteSaleHandler(ISaleRepository saleRepository) : IRequestHandler<DeleteSaleCommand, DeleteSaleResponse>
{
    public async Task<DeleteSaleResponse> Handle(DeleteSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        // Cancel instead of physical delete
        sale.Cancel();
        await saleRepository.UpdateAsync(sale, cancellationToken);

        return new DeleteSaleResponse { Success = true };
    }
}
