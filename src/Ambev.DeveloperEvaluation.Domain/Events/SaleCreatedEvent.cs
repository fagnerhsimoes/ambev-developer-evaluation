using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleCreatedEvent(Sale sale)
{
    public Sale Sale { get; } = sale;
}
