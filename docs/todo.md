# Loan Management System — Implementation Checklist (TODO)

Use this checklist with your AI coding agent (e.g. OpenCode) to track progress task by task. Check off items `[x]` as they are completed.

---

## Phase 1: Solution Scaffolding & Architecture Setup
- [x] **1.1 Solution Projects Setup**
  - [x] Create `LoanManagement.Domain` class library (`net10.0`).
  - [x] Create `LoanManagement.Application` class library (`net10.0`).
  - [x] Create `LoanManagement.Infrastructure` class library (`net10.0`).
  - [x] Rename / structure `LoanManagement.API` Web API project (`net10.0`).
  - [x] Configure project references adhering to Clean Architecture dependency rules.
- [x] **1.2 Core Dependencies Installation**
  - [x] Install `MediatR` and `FluentValidation` in `Application`.
  - [x] Install `Microsoft.EntityFrameworkCore.SqlServer` (or Npgsql) and `Microsoft.EntityFrameworkCore.Design` in `Infrastructure`.
  - [x] Install `Serilog.AspNetCore`, `Swashbuckle.AspNetCore` / `Microsoft.AspNetCore.OpenApi` in `API`.

---

## Phase 2: Domain Layer (Pure C# & Financial Core)
- [x] **2.1 Base Primitives**
  - [x] Implement `BaseEntity` (Id, CreatedAt, CreatedBy, UpdatedAt, IsDeleted).
  - [x] Implement `IAggregateRoot` and `IDomainEvent`.
  - [x] Implement `Result<T>` pattern and Domain Exceptions (`DomainValidationException`, `InvalidLoanStateException`).
- [x] **2.2 Domain Entities & Enums**
  - [x] Enums: `LoanStatus`, `RepaymentStatus`, `RiskRating`, `TransactionType`.
  - [x] Entities: `Organization`, `Borrower`, `LoanProduct`, `Loan`, `RepaymentScheduleItem`, `RepaymentPayment`, `GlAccount`, `GlJournalEntry`, `GlJournalLine`, `KycDocument`, `CreditCheck`, `Dispute`, `AuditLog`.
- [x] **2.3 Financial Calculation Services**
  - [x] Implement Equal Monthly Installment (EMI) amortization schedule calculator using `decimal`.
  - [x] Implement daily interest accrual calculation logic.

---

## Phase 3: Infrastructure & Persistence
- [x] **3.1 EF Core DbContext & Entity Configurations**
  - [x] Create `ApplicationDbContext` implementing `IApplicationDbContext`.
  - [x] Create EF Core entity configurations using Fluent API (set column types `decimal(18,4)`, indexes, foreign keys).
  - [x] Configure `AuditableEntityInterceptor` for auto-populating timestamps and user audit data.
- [x] **3.2 Database Migrations**
  - [x] Generate initial database migration (`InitialLoanManagementSchema`).
  - [x] Create seed data for Default Organization, Loan Products, Chart of Accounts (GL Accounts), and test users.
- [x] **3.3 External Service Mock Implementations**
  - [x] Implement `CreditBureauService` (Mock 3rd-party credit agency).
  - [x] Implement local/blob `DocumentStorageService` for KYC documents.
  - [x] Implement database-backed `IdempotencyService`.

---

## Phase 4: Application Layer (CQRS & Business Logic)
- [x] **4.1 MediatR Pipeline Behaviors**
  - [x] Implement `ValidationBehavior<TRequest, TResponse>`.
  - [x] Implement `LoggingBehavior<TRequest, TResponse>`.
  - [x] Implement `TransactionBehavior<TRequest, TResponse>`.
- [x] **4.2 Loan Origination & Underwriting Commands**
  - [x] `CreateLoanCommand` + Handler + FluentValidation rules.
  - [x] `UploadKycDocumentCommand` + Handler.
  - [x] `RunCreditCheckCommand` + Handler.
  - [x] `ApproveLoanCommand` + Handler (generates schedule and changes status).
- [x] **4.3 Servicing & Financial Operations**
  - [x] `DisburseLoanCommand` + Handler (posts initial GL entries).
  - [x] `MakeRepaymentCommand` + Handler (allocates payment to principal/interest, updates balance, posts GL entries).
  - [x] `ApplyTopUpCommand` + Handler.
  - [x] `ResolveCustomerDisputeCommand` + Handler.
- [x] **4.4 Queries**
  - [x] `GetPaymentScheduleQuery` + Handler.
  - [x] `GetDelinquentLoansQuery` + Handler (paginated).
  - [x] `GetLoanStatementQuery` + Handler.
  - [x] `GetAuditLogsQuery` + Handler.
  - [x] `ExportPortfolioReportQuery` + Handler.
- [x] **4.5 Accounting & Batch Operations**
  - [x] `PostGlJournalEntryCommand` + Handler (double-entry validation: sum(Debit) == sum(Credit)).
  - [x] `UploadBankReconciliationCommand` + Handler.
  - [x] `GenerateDailyAccrualsCommand` + Handler.
  - [x] `RunBatchBillingCommand` + Handler.
  - [x] `ManageProductRatesCommand` + Handler.
  - [x] `TriggerProvisioningChargeCommand` + Handler (IFRS 9 ECL model).
  - [x] `BulkImportLoansCommand` + Handler.

---

## Phase 5: API Layer, Middlewares & Security
- [x] **5.1 Middlewares**
  - [x] Implement `ExceptionHandlingMiddleware` for standardized RFC 7807 `ProblemDetails`.
  - [x] Implement `IdempotencyMiddleware` (inspects and enforces `Idempotency-Key` header).
- [x] **5.2 Authentication & Multi-Tenancy**
  - [x] Configure JWT Bearer authentication with Role-Based Access Control (RBAC).
  - [x] Implement tenant isolation filter/middleware validating `organizationId`.
- [x] **5.3 Controllers Implementation**
  - [x] `LoansController` (APIs 01, 02, 03, 04, 05, 07, 12, 16, 17, 18).
  - [x] `RepaymentsController` (API 06).
  - [x] `BorrowersController` (API 09).
  - [x] `AccountingController` (APIs 11, 15).
  - [x] `BatchController` (APIs 10, 19).
  - [x] `RiskController` (APIs 08, 14).
  - [x] `ProductsController` (API 13).
  - [x] `AuditController` (API 20).
- [x] **5.4 Swagger / OpenAPI Setup**
  - [x] Configure Swagger UI with JWT Bearer auth support and `Idempotency-Key` header parameter.

---

## Phase 6: Background Jobs (Automations)
- [x] **6.1 Daily Interest Accruals**
  - [x] Configure `DailyInterestAccrualJob` to run automatically at 00:00 UTC daily.
- [x] **6.2 Monthly Statement Billing**
  - [x] Configure `MonthlyBillingJob` to run on the 1st of every month.

---

## Phase 7: Automated Tests & Verification
- [x] **7.1 Unit Tests (`LoanManagement.UnitTests`)**
  - [x] Test EMI Amortization calculations for various principal amounts and tenors.
  - [x] Test Repayment split allocation (Principal vs Interest vs Late Fees).
  - [x] Test GL double-entry rule (Debit equals Credit validation).
  - [x] Test Daily Interest Accrual formula.
- [x] **7.2 Integration Tests (`LoanManagement.IntegrationTests`)**
  - [x] Test idempotency behavior (submitting duplicate payment returns cached result).
  - [x] Test complete loan lifecycle end-to-end: Create -> Approve -> Disburse -> Repay.

---

## Phase 8: Final Polish & Portfolio Readiness
- [x] Update root `README.md` with Architecture Diagrams, System Overview, Setup Guide, and Swagger instructions.
- [x] Verify clean build with zero warnings: `dotnet build`.
- [x] Verify all tests pass: `dotnet test`.

## Phase 9: Auth, User Management & Docker (New Tasks)
- [x] **9.1 Fix Token Role Bug**
  - [x] Sanitize /api/v1/auth/token to ignore invalid "string" roles and return valid system roles.
- [x] **9.2 Full Auth & User Management**
  - [x] Add User entity (Id, Email, PasswordHash, FullName, Role, OrganizationId, IsActive, CreatedAt).
  - [x] Implement password hashing (PBKDF2/HMACSHA256) via IPasswordHasher; no plaintext passwords.
  - [x] Seed test users: dmin@bank.com/Admin123!, officer@bank.com/Officer123!, underwriter@bank.com/Underwriter123!, ops@bank.com/Ops123!, customer@bank.com/Customer123!.
  - [x] Add POST /api/v1/auth/register and POST /api/v1/auth/login (JWT with real claims/roles).
  - [x] Add EF migration for Users/auth seed and apply migrations automatically on API startup.
- [x] **9.3 Dockerfile & Docker Compose**
  - [x] Update src/LoanManagement.API/Dockerfile to multi-stage .NET 10 build.
  - [x] Add root docker-compose.yml with sqlserver (2022-latest, SA password, 1433) and loan-management-api (depends on sqlserver, port 8080, connection string env).
  - [x] Verify API container startup applies migrations (Database.Migrate()).
