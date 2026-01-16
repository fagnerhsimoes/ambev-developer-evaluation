using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleCancelledEvent(Sale sale)
{
    public Sale Sale { get; } = sale;
}
