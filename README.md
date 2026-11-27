# Loan Management System

> **Status: Under Development 🚧**  
> This project is actively being developed. Core loan management features are implemented, and additional features/improvements will be added incrementally (see `docs/todo.md` backlog).

A production-grade **Loan Management System** backend built with **.NET 10 / ASP.NET Core**, designed for FinTech/banking workflows. The solution follows **Clean Architecture**, **CQRS (MediatR)**, **DDD-inspired domain modeling**, and enforces financial integrity rules (decimal precision, idempotency, double-entry GL, auditability, RBAC, multi-tenancy).

## Key Features

- **Auth & Users**: register/login, JWT Bearer auth, RBAC roles, seeded dev users, dev token endpoint (sanitized roles).
- **Loan origination & underwriting**: create loan applications, KYC upload, credit check, approval with amortization schedule generation.
- **Servicing & repayments**: disbursement, repayment allocation (interest/principal), top-ups, disputes, delinquency tracking.
- **Accounting & GL**: double-entry journal posting, disbursement/repayment GL entries, provisioning charge posting, bank reconciliation.
- **Batch automation**: Quartz jobs for daily interest accrual (00:00 UTC) and monthly billing (1st of month).
- **Risk & reporting**: portfolio export, provisioning charges, audit log querying.
- **Security & compliance**: JWT auth, RBAC policies, tenant isolation, idempotency middleware, RFC 7807 error responses.

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

## Tech Stack

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core 10 (SQL Server provider)
- MediatR, FluentValidation
- Quartz.NET (background jobs)
- Swashbuckle / OpenAPI
- xUnit, FluentAssertions, EF Core InMemory
- Docker / Docker Compose

## Configuration (.env)

The project supports a `.env` file (loaded by `DotNetEnv`). Example:

```env
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:8080

ConnectionStrings__DefaultConnection=Server=(localdb)\MSSQLLocalDB;Database=LoanManagementDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true
Jwt__Key=DEV_ONLY_SUPER_SECRET_KEY_CHANGE_ME_1234567890

MSSQL_SA_PASSWORD=Your_password123
```

> Use `.env.example` as a template. Do not commit real production secrets.

## Getting Started (Local)

1. **Restore & build**
   ```bash
   dotnet build Loan_Management_System.slnx
   ```

2. **Configure environment**
   - Copy `.env.example` to `.env` and adjust values if needed.

3. **Apply migrations** (optional; API also applies migrations on startup)
   ```bash
   dotnet ef database update --project src/LoanManagement.Infrastructure --startup-project src/LoanManagement.API
   ```

4. **Run API**
   ```bash
   dotnet run --project src/LoanManagement.API
   ```

5. **Endpoints**
   - Root: `GET /` returns a simple API status response (no Swagger redirect).
   - Swagger UI: `/swagger`

## Run with Docker Compose

```bash
docker compose up --build
```

Services:
- `sqlserver` - SQL Server 2022 (`1433`)
- `loan-management-api` - API (`8080`)

The API applies EF Core migrations automatically on startup.

## Authentication (Dev/Test)

- `POST /api/v1/auth/register` - register user
- `POST /api/v1/auth/login` - login and receive JWT
- `POST /api/v1/auth/token` - dev token generation (sanitizes invalid roles like `"string"`)

Seeded dev users:
- `admin@bank.com` / `Admin123!`
- `officer@bank.com` / `Officer123!`
- `underwriter@bank.com` / `Underwriter123!`
- `ops@bank.com` / `Ops123!`
- `customer@bank.com` / `Customer123!`

## Testing

```bash
dotnet test Loan_Management_System.slnx
```

## Roadmap / Under Development

This repository is still under development. Planned and in-progress enhancements are tracked in `docs/todo.md` (including backlog items for security hardening, observability, API platform improvements, reliability, and DevOps quality).

## Security Notes

- **Do not use default JWT key or SA password in production**.
- Use secret management/environment-specific config for production.
- Enforce HTTPS, secret rotation, and a proper identity provider before production deployment.