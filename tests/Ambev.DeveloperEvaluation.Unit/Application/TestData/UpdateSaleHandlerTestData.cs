using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class UpdateSaleHandlerTestData
{
    private static readonly Faker<UpdateSaleCommand.UpdateSaleItemDto> UpdateSaleItemDtoFaker = new Faker<UpdateSaleCommand.UpdateSaleItemDto>()
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 20))
        .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(10, 100));

    private static readonly Faker<UpdateSaleCommand> UpdateSaleCommandFaker = new Faker<UpdateSaleCommand>()
        .RuleFor(c => c.Id, f => f.Random.Guid())
        .RuleFor(c => c.SaleNumber, f => f.Commerce.Ean13())
        .RuleFor(c => c.Date, f => f.Date.Past())
        .RuleFor(c => c.CustomerId, f => f.Random.Guid())
        .RuleFor(c => c.CustomerName, f => f.Name.FullName())
        .RuleFor(c => c.BranchId, f => f.Random.Guid())
        .RuleFor(c => c.BranchName, f => f.Company.CompanyName())
        .RuleFor(c => c.IsCancelled, f => f.Random.Bool())
        .RuleFor(c => c.Items, f => UpdateSaleItemDtoFaker.Generate(3));

    public static UpdateSaleCommand GenerateValidCommand()
    {
        return UpdateSaleCommandFaker.Generate();
    }
}
