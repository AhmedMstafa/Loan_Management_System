# AGENTS.md — Project Rules (Loan Management System)

## 0) Project Facts & Stack
- **System**: Loan Management System (FinTech / Banking Domain)
- **Stack**: C# (.NET 10 / ASP.NET Core Web API), Entity Framework Core, SQL Server / PostgreSQL
- **Architecture**: Clean Architecture + CQRS (MediatR) + Domain-Driven Design (DDD) principles
- **Core Libraries**: MediatR, FluentValidation, Mapster / AutoMapper, Serilog, Quartz.NET / BackgroundService
- **Testing**: xUnit, FluentAssertions, Moq, Respawn / Testcontainers
- **Commands**:
  - Build: `dotnet build`
  - Run: `dotnet run --project Loan_Management_System`
  - Test: `dotnet test`
- **Docs Reference**: Always follow `docs/` (`docs/api-routes.md`, `docs/backend-structure.md`, `docs/db-schema.md`, `docs/todo.md`).

---

## 1) FinTech & Banking Critical Rules (MUST FOLLOW)
1. **Financial Precision**:
   - Always use `decimal` for all monetary amounts, interest rates, balances, and calculations.
   - NEVER use `float` or `double` for currency or interest.
   - Use standard banking rounding: `MidpointRounding.ToEven` when rounding cents.
2. **Idempotency**:
   - All mutating financial operations (Repayments, Disbursements, Approvals, Disputes, Top-ups) MUST enforce the `Idempotency-Key` header.
   - Return previous cached response if key was already processed; never charge or disburse twice.
3. **Double-Entry & Ledger Integrity**:
   - Every financial disbursement and repayment must produce corresponding immutable General Ledger (GL) debit/credit entries.
   - Balances must be verifiable by summing ledger transactions.
4. **Audit Trail & Immutability**:
   - Status changes (Application -> Approved -> Disbursed -> Delinquent -> Closed) and security actions must emit Domain Events and write an immutable record to `AuditLogs`.

---

## 2) Architecture & Layer Boundaries
- **Domain**: Pure C#. Contains Entities, Value Objects, Enums, Domain Exceptions, and Domain Events. ZERO dependencies on EF Core, ASP.NET Core, or external packages.
- **Application**: CQRS Commands, Queries, Handlers (MediatR), DTOs, Validators (FluentValidation), and Interfaces. No direct database or external protocol code.
- **Infrastructure**: EF Core DbContext, Configurations, Migrations, Repositories, 3rd-party integrations (Credit Check, KYC storage, Notifications), Background Jobs.
- **API**: Controllers / Minimal Endpoints, Middlewares (ExceptionHandling, Idempotency, CorrelationId), Authentication & Authorization policies.

---

## 3) API Design & Error Handling
- Use proper HTTP verbs:
  - `GET` for retrieval (with pagination: `pageNumber`, `pageSize`, filtering).
  - `POST` for resource creation and non-idempotent action triggers.
  - `PUT` / `PATCH` for updates.
- All errors must return RFC 7807 `ProblemDetails` with clear error codes and field-level validation messages.
- Never swallow exceptions or return generic `500` without logging details.

---

## 4) Security & RBAC
- Validate `organizationId` for multi-tenant isolation on every request.
- Enforce Role-Based Access Control (RBAC) via attributes/policies:
  - `LoanOfficer`, `Underwriter`, `Compliance`, `BackOffice`, `Accountant`, `Collector`, `RiskAnalyst`, `Customer`, `Admin`.
- Never expose sensitive borrower PII (SSN, national ID, bank account details) in plain text logs.

---

## 5) Workflow & Implementation Guidance
- Follow the task order in `docs/todo.md`.
- Read `docs/api-routes.md` before implementing any endpoint.
- Read `docs/db-schema.md` before modifying entities or migrations.
- When working on a feature, update the task status in `docs/todo.md`.

| Trigger | Read & follow |
|---|---|
| Starting a new feature | `.ai/skills/feature-scaffolding.md` |
| Before saying a task is done | `.ai/skills/code-review.md` |
| Branch / commit / PR | `.ai/skills/git-pr.md` |
| Bug, crash, failing test | `.ai/agents/debugger.md` |
| Need a deeper independent review | `.ai/agents/code-reviewer.md` |
| Code changed without tests | `.ai/agents/test-writer.md` |
| Database schema modification | `docs/db-schema.md` |
