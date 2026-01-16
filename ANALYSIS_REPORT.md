# Code Review & Refactoring Report - WebApi Layer

This report identifies architectural issues, "traps," and technical debt discovered in the
`Ambev.DeveloperEvaluation.WebApi` project, specifically within the Controllers and Middleware integration.

## 1. Redundant Validation (DRY Violation)

**Location:** All methods in `UsersController` and `SalesController`.
**Issue:** Controllers are manually instantiating validators and checking `validationResult.IsValid`.
**Why it's a "trap":**

- The project already has a `ValidationBehavior` (MediatR Pipeline) and a `ValidationExceptionMiddleware`.
- Validation should be automatic. When `_mediator.Send(command)` is called, the pipeline triggers validation. If it
  fails, the middleware catches the exception and returns a formatted 400 response.
  **Impact:** Duplicate code, inconsistent error responses, and higher maintenance cost.

## 2. Inconsistent Validation Patterns

**Location:** `SalesController.GetSales` vs others.
**Issue:** While most endpoints have manual validation, `GetSales` skips it and sends the command directly to the
mediator.
**Observation:** Ironically, `GetSales` is the closest to the "clean" architecture intended for this project, while the
others are cluttered with legacy-style checks.

## 3. Route vs. Body ID Mismatch

**Location:** `SalesController.UpdateSale`
**Issue:** Manual check for `id != request.Id`.
**Recommendation:** The ID from the route should be considered the "source of truth." The Command should be populated
with the Route ID, and any consistency checks should happen within the FluentValidation rules for the Command, not as an
`if` statement in the Controller.

## 4. Over-engineering Simple Requests (GET/DELETE)

**Location:** `GetUser`, `DeleteUser`, `GetSale`, `DeleteSale`.
**Issue:** Creating a Request DTO + Validator + Command just to pass a single `Guid`.
**Recommendation:** For simple operations, the Controller can directly create the Command from the route parameter. If
the GUID format is invalid, ASP.NET Core Model Binding handles it. If the ID doesn't exist, the Application Handler
should return a "Not Found" result or throw a specific exception.

## 5. Security Gap (Authentication/Authorization)

**Location:** All Controllers.
**Issue:** `app.UseAuthentication()` and `app.UseAuthorization()` are present in `Program.cs`, but the Controllers lack
`[Authorize]` attributes.
**Impact:** The API is currently public. Anyone can perform CRUD operations on Users and Sales without a valid JWT
token.

## 6. Manual Mapping Logic

**Location:** `SalesController.GetSales`
**Issue:** Manual instantiation of `PaginatedList<T>` inside the controller.
**Recommendation:** This mapping should be handled by AutoMapper using a specific profile for pagination or moved to the
Application Layer so the Controller only receives the final DTO.

---

## Next Steps for Refactoring:

1. **Clean Controllers:** Remove all manual validation blocks and validator instantiations.
2. **Standardize Error Handling:** Ensure `ValidationExceptionMiddleware` produces the same JSON structure currently
   returned by `BadRequest(errors)`.
3. **Secure Endpoints:** Apply `[Authorize]` and `[Roles]` where appropriate.
4. **Simplify Mappings:** Move complex mapping logic to AutoMapper profiles.
