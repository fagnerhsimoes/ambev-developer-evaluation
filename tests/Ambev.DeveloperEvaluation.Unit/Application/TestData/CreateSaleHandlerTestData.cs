using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class CreateSaleHandlerTestData
{
    private static readonly Faker<CreateSaleCommand.CreateSaleItemDto> CreateSaleItemDtoFaker = new Faker<CreateSaleCommand.CreateSaleItemDto>()
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 20))
        .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(10, 100));

    private static readonly Faker<CreateSaleCommand> CreateSaleCommandFaker = new Faker<CreateSaleCommand>()
        .RuleFor(c => c.SaleNumber, f => f.Commerce.Ean13())
        .RuleFor(c => c.Date, f => f.Date.Past())
        .RuleFor(c => c.CustomerId, f => f.Random.Guid())
        .RuleFor(c => c.CustomerName, f => f.Name.FullName())
        .RuleFor(c => c.BranchId, f => f.Random.Guid())
        .RuleFor(c => c.BranchName, f => f.Company.CompanyName())
        .RuleFor(c => c.Items, f => CreateSaleItemDtoFaker.Generate(3));

    public static CreateSaleCommand GenerateValidCommand()
    {
        return CreateSaleCommandFaker.Generate();
    }
}
