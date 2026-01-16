# Implementation Report - Sales Module

This document summarizes the implementations carried out in the Sales module (`Sales`), mapping the delivered features
against the requirements and established architectural patterns.

## 1. Architecture and Patterns

The development strictly followed the **Clean Architecture** and **DDD (Domain-Driven Design)** principles already
existing in the project.

* **Layers**:
    * `Domain`: Entities (`Sale`, `SaleItem`), Enums (`SaleStatus`), Events (`SaleCreated`, `SaleModified`),
      Interfaces (`ISaleRepository`).
    * `Application`: Use Cases (Handlers), DTOs, Mappings (AutoMapper), Validation (FluentValidation - with external
      user validation).
    * `ORM`: Repository Implementation with Entity Framework Core.
    * `WebApi`: Controllers, ViewModels, `PaginatedList` handling.
* **Patterns**:
    * **CQS/CQRS**: Use of `MediatR` to separate Commands and Queries.
    * **Repository Pattern**: Isolation of data access.
    * **Soft Delete**: Logical deletion implemented via `Cancel()` method and status update.
    * **Notification Pattern**: Publishing domain events (`SaleCreatedEvent`, `SaleModifiedEvent`) for decoupling.

## 2. Functional Requirements Implemented

### 2.1. Create Sale (`POST /api/sales`)

* **User Validation**: Verification if `CustomerId` exists in the users service (External Identity Pattern).
* **Discount Rules**:
    *   [x] Purchases above 4 identical items: **10% discount**.
    *   [x] Purchases between 10 and 20 identical items: **20% discount**.
    *   [x] **Restriction**: Not allowed to sell more than 20 identical items.
    *   [x] **Restriction**: Not allowed to sell less than 4 items (no discount, standard logic applied).
* **Calculation**: Total sale amount calculated automatically by summing item subtotals.
* **Events**: Triggering of `SaleCreatedEvent` upon success.

### 2.2. Update Sale (`PUT /api/sales/{id}`)

* **Recalculation**: When changing items, all totals and discounts are recalculated.
* **Events**: Triggering of `SaleModifiedEvent` with updated data.
* **Audit**: Update of `UpdatedAt` field.

### 2.3. Get Sale (`GET /api/sales/{id}`)

* Returns full sale details, including items, applied discounts, and current status.

### 2.4. List Sales (`GET /api/sales`)

* **Pagination**: Implemented supporting `_page`, `_size`, and `_order` parameters.
* **Architecture**: Uses `GetSalesResult` in Application and maps to `PaginatedList` in WebApi, following the
  responsibility separation pattern.

### 2.5. Cancel/Delete (`DELETE /api/sales/{id}`)

* **Soft Delete**: Implemented through the `Cancel()` method.
* **Integrity**: The physical `DeleteAsync` method was removed from the repository to ensure sales are never physically
  deleted, only canceled.

### 2.6. Validation Refactoring

I identified that ValidationBehavior was already registered in IoC, but it wasn't being used. I removed the
manual/explicit validation from the Controllers and Handlers to ensure the DRY principle and correctly utilize the
MediatR Pipeline, centralizing error handling in the existing Middleware.

## 3. Senior-Level Improvements and Refactoring

* **SaleStatus Enum**: Replaced boolean flags with an Enum (`Pending`, `Completed`, `Cancelled`) for better state
  management.
* **PaginatedList Refactoring**: Removed duplicates and restored the original pattern in the `WebApi` layer.
* **Code Cleanup**: Removed unused variables and unnecessary imports.

## 4. Tests and Quality

* **Unit Tests**: Test coverage implemented in the `Ambev.DeveloperEvaluation.Unit` project.
    * `SaleHandlerTests`: Validation of create, update, and delete flows.
    * `SaleRepositoryTests`: Persistence validation.
    * **Total**: 99 Passing Tests (0 failures).

## 5. API Documentation

* Generated file: [.doc/sales-api.md](.doc/sales-api.md) containing JSON examples for Request/Response of all endpoints.

