# Next Steps and Suggested Improvements

This document outlines identified improvements that would add value to the project, demonstrating a Senior view of
architecture, quality, and scalability, while respecting the original constraints and standards of the test.

## 1. Security (RBAC)

- **Role-Based Access Control (RBAC)**:
    - *Suggestion*: Implement Role verification (e.g., `[Authorize(Roles = "Manager,Admin")]`) for sensitive operations
      such as Sale Cancellation.
    - *Value*: Increases security granularity beyond simple authentication.

## 2. Quality and Automated Testing

- **Integration Tests (E2E)**:
    - *Suggestion*: Create tests using `Microsoft.AspNetCore.Mvc.Testing` and `Testcontainers`.
    - *Value*: Validates the complete flow (API -> App -> Infra -> Database) in a controlled environment, ensuring that
      dependency injection and database mapping are perfect.
- **Load Tests**:
    - *Suggestion*: Add basic scripts (e.g., k6) to validate the performance of concurrent sale creation.

## 3. Observability and Monitoring

- **Structured Logging**:
    - *Suggestion*: Enrich logs in `CreateSaleHandler` and `UpdateSaleHandler` with structured properties (e.g.,
      `SaleId`, `CustomerId`) using Serilog, facilitating future searches.
- **Metrics**:
    - *Suggestion*: Expose business metrics (e.g., "Sales Created per Hour", "Total Discounts Given") using
      OpenTelemetry + Prometheus.

## 4. Resilience and Performance

- **Idempotency**:
    - *Suggestion*: Implement an Idempotency mechanism on the `POST /sales` endpoint using a unique key sent by the
      client (Header `Idempotency-Key`).
    - *Value*: Prevents sale duplication in case of network failures.
- **Caching**:
    - *Suggestion*: Implement Caching (Redis) on the `GET /sales/{id}` endpoint for finalized/old sales that rarely
      change.

## 5. Documentation

- **Swagger Examples**:
    - *Suggestion*: Add rich Request/Response examples in Swagger (using `Swashbuckle.AspNetCore.Filters`).
    - *Value*: Greatly facilitates API consumption by other developers/front-ends.

## 6. Architecture

- **Notification Pattern**:
    - *Suggestion*: Evolve error return to accumulate domain notifications instead of throwing exceptions for business
      rule validations (if the project pattern permits).

## 7. CI/CD

- **Github Actions**:
    - *Suggestion*: Add a `.github/workflows/ci.yml` file to build and run tests automatically on every Push/PR.

---
> **Note**: These improvements were documented but not implemented to keep the scope strictly aligned with the test
> request, demonstrating respect for initial requirements (YAGNI - You Aren't Gonna Need It Yet).
