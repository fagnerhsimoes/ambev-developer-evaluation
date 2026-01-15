# Implementation Plan - Sales Module

## User Review Required

> [!IMPORTANT]
> **External Identity Pattern**: I will apply the External Identity pattern by denormalizing `Customer` and `Branch`
> data into the `Sale` entity. Specifically, I will store `CustomerId` (Guid) and `CustomerName` (string), and similarly
> for `Branch`. This avoids synchronous calls to other services/modules during reads.

> [!NOTE]
> **Discount Logic**: The business rules specify discounts for *identical items*. I will implement this logic within the
`Sale` aggregation root or `SaleFactory`/`Manager` domain service to ensure consistency. The logic will apply:
> - 10% discount for 4+ items.
> - 20% discount for 10-20 items.
> - Restriction: Max 20 items per product.
> - Restriction: No discount < 4 items.

## Proposed Changes

### Domain Layer (Ambev.DeveloperEvaluation.Domain)

I will create the `Sales` aggregate.

#### [NEW] [Sale.cs](file:///home/fagner/Documents/Tests/ambev-developer-evaluation/src/Ambev.DeveloperEvaluation.Domain/Entities/Sale.cs)

- Properties: `SaleNumber`, `Date`, `CustomerId`, `CustomerName`, `BranchId`, `BranchName`, `TotalAmount`,
  `IsCancelled`.
- Collection: `List<SaleItem> SaleItems`.
- Methods: `Cancel()`, `Update()`.

#### [NEW] [SaleItem.cs](file:///home/fagner/Documents/Tests/ambev-developer-evaluation/src/Ambev.DeveloperEvaluation.Domain/Entities/SaleItem.cs)

- Properties: `ProductId`, `ProductName`, `Quantity`, `UnitPrice`, `Discount`, `TotalAmount`, `IsCancelled`.
- Logic: `CalculateTotal()` (applies discount based on quantity).

#### [NEW] Events

- `SaleCreatedEvent`
- `SaleModifiedEvent`
- `SaleCancelledEvent`
- `ItemCancelledEvent`

#### [NEW] Repositories Interface

- `ISaleRepository`

### Application Layer (Ambev.DeveloperEvaluation.Application)

I will implement CQRS pattern for Sales.

#### [NEW] Sales/CreateSale

- `CreateSaleCommand` (DTO)
- `CreateSaleHandler`:
    - Validate request.
    - specific domain logic for discounts.
    - Persist `Sale`.
    - Publish `SaleCreatedEvent`.
- `CreateSaleValidator`: Max 20 items validation.
- `CreateSaleResult`

#### [NEW] Sales/GetSale

- `GetSaleCommand`
- `GetSaleHandler`
- `GetSaleResult`

#### [NEW] Sales/UpdateSale

- `UpdateSaleCommand`
- `UpdateSaleHandler`
    - Should handle `SaleModifiedEvent`.

#### [NEW] Sales/DeleteSale (Cancel)

- `DeleteSaleCommand`
- `DeleteSaleHandler`
    - Logic to cancel sale (soft delete or status update).
    - Publish `SaleCancelledEvent`.

### Infrastructure Layer (Ambev.DeveloperEvaluation.ORM)

#### [NEW] Repositories

- `SaleRepository`: Implementation of `ISaleRepository`.

#### [NEW] Mapping

- `SaleConfiguration`: EF Core configuration (table `Sales`).
- `SaleItemConfiguration`: EF Core configuration (table `SaleItems`).

### Web API Layer (Ambev.DeveloperEvaluation.WebApi)

#### [NEW] Features/Sales

- `SalesController`:
    - `POST /api/sales`
    - `GET /api/sales/{id}`
    - `PUT /api/sales/{id}`
    - `DELETE /api/sales/{id}`
- Requests/Responses DTOs matching the `Application` layer results.

### Tests (Ambev.DeveloperEvaluation.UnitTests)

I will check the existing test project location first, assuming `tests/Ambev.DeveloperEvaluation.UnitTests`.

#### [NEW] Domain Tests

- `SaleTests`: Validate discount logic (4 items=10%, 10-20 items=20%).
- `SaleTests`: Validate max 20 items exception.
- `SaleTests`: Validate no discount < 4 items.

## Verification Plan

### Automated Tests

Run unit tests to verify business logic:

```bash
dotnet test tests/Ambev.DeveloperEvaluation.UnitTests
```

### Manual Verification

1. **Create Sale**: Use Swagger to POST a generic sale. Verify 200 OK.
2. **Discount Logic**: Create sale with 5 items. Verify 10% discount in Total.
3. **Discount Logic**: Create sale with 15 items. Verify 20% discount.
4. **Restriction**: Create sale with 21 items. Verify 400 Bad Request (or Domain Exception).
5. **Get Sale**: GET the created sale ID. Verify correct data (including denormalized names).
6. **Cancel Sale**: DELETE the sale ID. Verify verification status.
