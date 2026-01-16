using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleModifiedEvent(Sale sale)
{
    public Sale Sale { get; } = sale;
}
