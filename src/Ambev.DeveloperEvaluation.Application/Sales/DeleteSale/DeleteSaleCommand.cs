using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Command for deleting (cancelling) a sale.
/// </summary>
public class DeleteSaleCommand : IRequest<DeleteSaleResponse>
{
    /// <summary>
    /// The unique identifier of the sale to delete.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Initializes a new instance of DeleteSaleCommand.
    /// </summary>
    /// <param name="id">The ID of the sale to delete.</param>
    public DeleteSaleCommand(Guid id)
    {
        Id = id;
    }
}
