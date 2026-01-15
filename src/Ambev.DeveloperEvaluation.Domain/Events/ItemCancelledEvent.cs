using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class ItemCancelledEvent
{
    public SaleItem SaleItem { get; }
    public Sale Sale { get; }

    public ItemCancelledEvent(SaleItem saleItem, Sale sale)
    {
        SaleItem = saleItem;
        Sale = sale;
    }
}
