using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an item in a sale.
/// Implements discount calculation logic based on quantity business rules.
/// </summary>
public class SaleItem : BaseEntity
{
    /// <summary>
    /// Gets or sets the sale identifier (Foreign Key).
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Gets or sets the product identifier (External Identity).
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product name.
    /// Denormalized from Product entity for performance.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity of products.
    /// Business Rule: Maximum 20 items per product.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product at the time of sale.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the discount percentage applied to this item (0-20%).
    /// Business Rules:
    /// - Quantity 1-3: 0% discount
    /// - Quantity 4-9: 10% discount
    /// - Quantity 10-20: 20% discount
    /// - Quantity > 20: Not allowed (validation error)
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Gets or sets the total amount for this item after discount.
    /// Formula: (Quantity * UnitPrice) * (1 - Discount/100)
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this item is cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Calculates the total amount and applies discount based on quantity.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when quantity exceeds 20 items or is less than 1.
    /// </exception>
    public void CalculateTotal()
    {
        // Validate quantity limits
        if (Quantity > 20)
        {
            throw new InvalidOperationException(
                $"Cannot sell more than 20 identical items. Current quantity: {Quantity}"
            );
        }

        if (Quantity < 1)
        {
            throw new InvalidOperationException(
                $"Quantity must be at least 1. Current quantity: {Quantity}"
            );
        }

        // Calculate discount based on business rules
        if (Quantity < 4)
        {
            Discount = 0m;
        }
        else if (Quantity >= 4 && Quantity < 10)
        {
            Discount = 10m;
        }
        else // Quantity >= 10 && Quantity <= 20
        {
            Discount = 20m;
        }

        // Calculate total with discount (Discount stored as percentage: 0, 10, 20)
        TotalAmount = Math.Round(Quantity * UnitPrice * (1 - Discount / 100), 2);
    }

    /// <summary>
    /// Cancels this item and sets total amount to zero.
    /// </summary>
    public void Cancel()
    {
        IsCancelled = true;
        TotalAmount = 0;
    }

    /// <summary>
    /// Validates the sale item entity using SaleItemValidator.
    /// </summary>
    /// <returns>Validation result with errors if any.</returns>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleItemValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}