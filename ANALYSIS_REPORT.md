# Code Review & Refactoring Report - WebApi Layer

This report identifies architectural issues, "traps," and technical debt discovered in the
`Ambev.DeveloperEvaluation.WebApi` project, specifically within the Legacy Controllers (e.g., `UsersController`) and
Middleware integration.

## 1. Redundant Validation (DRY Violation)

**Location:** Methods in `UsersController`.
**Issue:** Controllers are manually instantiating validators and checking `validationResult.IsValid`.
**Why it's a "trap":**

- The project already has a `ValidationBehavior` (MediatR Pipeline) and a `ValidationExceptionMiddleware`.
- Validation should be automatic. When `_mediator.Send(command)` is called, the pipeline triggers validation. If it
  fails, the middleware catches the exception and returns a formatted 400 response.
  **Impact:** Duplicate code, inconsistent error responses, and higher maintenance cost.

## 2. Over-engineering Simple Requests (GET/DELETE)

**Location:** `GetUser`, `DeleteUser`.
**Issue:** Creating a Request DTO + Validator + Command just to pass a single `Guid`.
**Recommendation:** For simple operations, the Controller can directly create the Command from the route parameter. If
the GUID format is invalid, ASP.NET Core Model Binding handles it. If the ID doesn't exist, the Application Handler
should return a "Not Found" result or throw a specific exception.

## 3. Security Gap (Authentication/Authorization)

**Location:** Controllers.
**Issue:** `app.UseAuthentication()` and `app.UseAuthorization()` are present in `Program.cs`, but the Controllers lack
`[Authorize]` attributes.
**Impact:** The API is currently public. Anyone can perform CRUD operations without a valid JWT
token.

---

## Next Steps for Refactoring:

1. **Clean Controllers:** Remove all manual validation blocks and validator instantiations.
2. **Standardize Error Handling:** Ensure `ValidationExceptionMiddleware` produces the same JSON structure currently
   returned by `BadRequest(errors)`.
3. **Secure Endpoints:** Apply `[Authorize]` and `[Roles]` where appropriate.
4. **Simplify Mappings:** Move complex mapping logic to AutoMapper profiles.
