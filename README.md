# Loan Management System

A production-grade **Loan Management System** backend built with **.NET 10 / ASP.NET Core**, designed for FinTech/banking workflows. The solution follows **Clean Architecture**, **CQRS (MediatR)**, **DDD-inspired domain modeling**, and enforces financial integrity rules (decimal precision, idempotency, double-entry GL, auditability, RBAC, multi-tenancy).

## Key Features

- **Loan origination & underwriting**: create loan applications, KYC upload, credit check, approval with amortization schedule generation.
- **Servicing & repayments**: disbursement, repayment allocation (interest/principal), top-ups, disputes, delinquency tracking.
- **Accounting & GL**: double-entry journal posting, disbursement/repayment GL entries, provisioning charge posting, bank reconciliation.
- **Batch automation**: Quartz jobs for daily interest accrual (00:00 UTC) and monthly billing (1st of month).
- **Risk & reporting**: portfolio export, provisioning charges, audit log querying.
- **Security & compliance**: JWT Bearer auth, RBAC policies, tenant isolation, idempotency middleware, RFC 7807 error responses.

## Architecture

```text
src/
  LoanManagement.Domain/          # Entities, enums, value objects, domain events, domain services, exceptions
  LoanManagement.Application/     # CQRS commands/queries, handlers, validators, interfaces, behaviors
  LoanManagement.Infrastructure/  # EF Core DbContext, configs, migrations, services, Quartz jobs
  LoanManagement.API/             # Controllers, middlewares, filters, auth, Swagger
tests/
  LoanManagement.UnitTests/
  LoanManagement.IntegrationTests/
docs/                             # api-routes, db-schema, backend-structure, todo
```

### Design Principles

- **Clean Architecture boundaries**: Domain has no external dependencies; Application depends on Domain; Infrastructure/API depend inward.
- **CQRS + MediatR**: commands/queries with validation, logging, and transaction pipeline behaviors.
- **Financial precision**: all monetary/interest values use `decimal` with `MidpointRounding.ToEven`.
- **Idempotency**: mutating financial operations support `Idempotency-Key` and cached replay protection.
- **Double-entry ledger**: journal entries validate `Debit == Credit` before posting.
- **Auditability**: auditable timestamps/user fields and audit log model for traceability.

## Tech Stack

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core 10 (SQL Server provider)
- MediatR, FluentValidation
- Quartz.NET (background jobs)
- Serilog (logging package configured)
- Swashbuckle / OpenAPI
- xUnit, FluentAssertions, EF Core InMemory

## Getting Started

### Prerequisites

- .NET SDK 10.x
- SQL Server / LocalDB (default connection uses `(localdb)\MSSQLLocalDB`)

### Setup

1. **Restore & build**
   ```bash
   dotnet build Loan_Management_System.slnx
   ```

2. **Configure database** (optional)
   Update `src/LoanManagement.API/appsettings.json` connection string `ConnectionStrings:DefaultConnection` if needed.

3. **Apply migrations**
   ```bash
   dotnet ef database update --project src/LoanManagement.Infrastructure --startup-project src/LoanManagement.API
   ```

4. **Run API**
   ```bash
   dotnet run --project src/LoanManagement.API
   ```

5. **Open Swagger UI**
   - Navigate to the Swagger endpoint shown in console output (typically `/swagger` in Development).
   - Use the **Authorize** button for JWT Bearer tokens.
   - For mutating financial endpoints, include `Idempotency-Key` header.

## Testing

```bash
dotnet test Loan_Management_System.slnx
```

Test coverage includes:
- Unit tests: amortization, daily interest accrual, repayment allocation, GL double-entry validation.
- Integration tests: idempotency replay behavior, end-to-end loan lifecycle (create -> approve -> disburse -> repay).

## API Modules

| Module | Examples |
|---|---|
| Loans | create, credit check, approve, disburse, schedule, statement, delinquent, top-up, dispute resolution, bulk import |
| Repayments | post repayment |
| Borrowers | KYC document upload |
| Accounting | bank reconciliation, GL journal posting |
| Batch | interest accruals, billing |
| Risk | portfolio export, provisioning charge |
| Products | manage product rates |
| Audit | query audit logs |

See `docs/api-routes.md` and `docs/db-schema.md` for detailed contracts and schema.

## Security Notes

- **Do not use default JWT key in production**; configure `Jwt:Key` via secure secrets/environment.
- Enforce HTTPS, secret rotation, and proper identity provider integration before production deployment.
- RBAC policies include roles such as `LoanOfficer`, `Underwriter`, `BackOffice`, `Accountant`, `RiskAnalyst`, `Admin`, etc.

## Development Workflow

The project tracks execution in `docs/todo.md`. Each completed phase is built, tested as required, committed, and pushed to `develop`.
