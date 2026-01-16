using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class ItemCancelledEvent(SaleItem saleItem, Sale sale)
{
    public SaleItem SaleItem { get; } = saleItem;
    public Sale Sale { get; } = sale;
}
