using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class SaleTestData
{
    private static readonly Faker<SaleItem> SaleItemFaker = new Faker<SaleItem>()
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.UnitPrice, f => f.Finance.Amount(10, 500))
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 20));

    private static readonly Faker<Sale> SaleFaker = new Faker<Sale>()
        .RuleFor(s => s.Id, f => f.Random.Guid())
        .RuleFor(s => s.SaleNumber, f => f.Commerce.Ean13())
        .RuleFor(s => s.SaleDate, f => f.Date.Past())
        .RuleFor(s => s.CustomerId, f => f.Random.Guid())
        .RuleFor(s => s.CustomerName, f => f.Name.FullName())
        .RuleFor(s => s.BranchName, f => f.Company.CompanyName())
        .RuleFor(s => s.Status, f => SaleStatus.Pending)
        .RuleFor(s => s.SaleItems, f => SaleItemFaker.Generate(3));

    public static Sale GenerateValidSale()
    {
        return SaleFaker.Generate();
    }

    public static SaleItem GenerateValidSaleItem(int quantity)
    {
        var item = SaleItemFaker.Generate();
        item.Quantity = quantity;
        return item;
    }

    public static SaleItem GenerateInvalidSaleItem(int quantity)
    {
        var item = SaleItemFaker.Generate();
        item.Quantity = quantity;
        return item;
    }
}
