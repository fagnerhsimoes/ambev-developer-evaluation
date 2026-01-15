using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale transaction.
/// This entity follows domain-driven design principles and includes business rules validation.
/// Implements External Identity Pattern for Customer, Branch, and Products.
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// Gets or sets the sale number.
    /// Must be unique across all sales.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date when the sale was made.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets or sets the customer identifier (External Identity).
    /// References User entity but stores only the ID.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the customer name.
    /// Denormalized from User entity for performance and audit.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch identifier (External Identity).
    /// References Branch entity (from another bounded context).
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Gets or sets the branch name.
    /// Denormalized from Branch entity for performance.
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total sale amount.
    /// Calculated from the sum of all sale items.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the status of the sale.
    /// </summary>
    public SaleStatus Status { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the sale is cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Gets the collection of sale items.
    /// </summary>
    public List<SaleItem> SaleItems { get; set; } = new();

    /// <summary>
    /// Gets or sets the date and time when the sale was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time of the last update.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the sale was cancelled.
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// Initializes a new instance of Sale.
    /// </summary>
    public Sale()
    {
        SaleDate = DateTime.UtcNow;
        Status = SaleStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates the total amount of the sale based on all items.
    /// </summary>
    public void CalculateTotalAmount()
    {
        TotalAmount = SaleItems.Where(i => !i.IsCancelled).Sum(i => i.TotalAmount);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the customer information for the sale (External Identity pattern).
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer.</param>
    /// <param name="customerName">The name of the customer.</param>
    public void SetCustomer(Guid customerId, string customerName)
    {
        CustomerId = customerId;
        CustomerName = customerName;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the branch information for the sale (External Identity pattern).
    /// </summary>
    /// <param name="branchId">The unique identifier of the branch.</param>
    /// <param name="branchName">The name of the branch.</param>
    public void SetBranch(Guid branchId, string branchName)
    {
        BranchId = branchId;
        BranchName = branchName;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the sale and all its items.
    /// </summary>
    public void Cancel()
    {
        if (IsCancelled) return;

        IsCancelled = true;
        Status = SaleStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        foreach (var item in SaleItems)
        {
            item.Cancel();
        }
    }

    /// <summary>
    /// Adds an item to the sale and recalculates the total.
    /// </summary>
    /// <param name="item">The sale item to add.</param>
    public void AddItem(SaleItem item)
    {
        SaleItems.Add(item);
        CalculateTotalAmount();
    }

    /// <summary>
    /// Performs validation of the sale entity using the SaleValidator rules.
    /// </summary>
    /// <returns>
    /// A <see cref="ValidationResultDetail"/> containing:
    /// - IsValid: Indicates whether all validation rules passed
    /// - Errors: Collection of validation errors if any rules failed
    /// </returns>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}