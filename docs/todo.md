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
- [ ] **3.1 EF Core DbContext & Entity Configurations**
  - [ ] Create `ApplicationDbContext` implementing `IApplicationDbContext`.
  - [ ] Create EF Core entity configurations using Fluent API (set column types `decimal(18,4)`, indexes, foreign keys).
  - [ ] Configure `AuditableEntityInterceptor` for auto-populating timestamps and user audit data.
- [ ] **3.2 Database Migrations**
  - [ ] Generate initial database migration (`InitialLoanManagementSchema`).
  - [ ] Create seed data for Default Organization, Loan Products, Chart of Accounts (GL Accounts), and test users.
- [ ] **3.3 External Service Mock Implementations**
  - [ ] Implement `CreditBureauService` (Mock 3rd-party credit agency).
  - [ ] Implement local/blob `DocumentStorageService` for KYC documents.
  - [ ] Implement database-backed `IdempotencyService`.

---

## Phase 4: Application Layer (CQRS & Business Logic)
- [ ] **4.1 MediatR Pipeline Behaviors**
  - [ ] Implement `ValidationBehavior<TRequest, TResponse>`.
  - [ ] Implement `LoggingBehavior<TRequest, TResponse>`.
  - [ ] Implement `TransactionBehavior<TRequest, TResponse>`.
- [ ] **4.2 Loan Origination & Underwriting Commands**
  - [ ] `CreateLoanCommand` + Handler + FluentValidation rules.
  - [ ] `UploadKycDocumentCommand` + Handler.
  - [ ] `RunCreditCheckCommand` + Handler.
  - [ ] `ApproveLoanCommand` + Handler (generates schedule and changes status).
- [ ] **4.3 Servicing & Financial Operations**
  - [ ] `DisburseLoanCommand` + Handler (posts initial GL entries).
  - [ ] `MakeRepaymentCommand` + Handler (allocates payment to principal/interest, updates balance, posts GL entries).
  - [ ] `ApplyTopUpCommand` + Handler.
  - [ ] `ResolveCustomerDisputeCommand` + Handler.
- [ ] **4.4 Queries**
  - [ ] `GetPaymentScheduleQuery` + Handler.
  - [ ] `GetDelinquentLoansQuery` + Handler (paginated).
  - [ ] `GetLoanStatementQuery` + Handler.
  - [ ] `GetAuditLogsQuery` + Handler.
  - [ ] `ExportPortfolioReportQuery` + Handler.
- [ ] **4.5 Accounting & Batch Operations**
  - [ ] `PostGlJournalEntryCommand` + Handler (double-entry validation: sum(Debit) == sum(Credit)).
  - [ ] `UploadBankReconciliationCommand` + Handler.
  - [ ] `GenerateDailyAccrualsCommand` + Handler.
  - [ ] `RunBatchBillingCommand` + Handler.
  - [ ] `ManageProductRatesCommand` + Handler.
  - [ ] `TriggerProvisioningChargeCommand` + Handler (IFRS 9 ECL model).
  - [ ] `BulkImportLoansCommand` + Handler.

---

## Phase 5: API Layer, Middlewares & Security
- [ ] **5.1 Middlewares**
  - [ ] Implement `ExceptionHandlingMiddleware` for standardized RFC 7807 `ProblemDetails`.
  - [ ] Implement `IdempotencyMiddleware` (inspects and enforces `Idempotency-Key` header).
- [ ] **5.2 Authentication & Multi-Tenancy**
  - [ ] Configure JWT Bearer authentication with Role-Based Access Control (RBAC).
  - [ ] Implement tenant isolation filter/middleware validating `organizationId`.
- [ ] **5.3 Controllers Implementation**
  - [ ] `LoansController` (APIs 01, 02, 03, 04, 05, 07, 12, 16, 17, 18).
  - [ ] `RepaymentsController` (API 06).
  - [ ] `BorrowersController` (API 09).
  - [ ] `AccountingController` (APIs 11, 15).
  - [ ] `BatchController` (APIs 10, 19).
  - [ ] `RiskController` (APIs 08, 14).
  - [ ] `ProductsController` (API 13).
  - [ ] `AuditController` (API 20).
- [ ] **5.4 Swagger / OpenAPI Setup**
  - [ ] Configure Swagger UI with JWT Bearer auth support and `Idempotency-Key` header parameter.

---

## Phase 6: Background Jobs (Automations)
- [ ] **6.1 Daily Interest Accruals**
  - [ ] Configure `DailyInterestAccrualJob` to run automatically at 00:00 UTC daily.
- [ ] **6.2 Monthly Statement Billing**
  - [ ] Configure `MonthlyBillingJob` to run on the 1st of every month.

---

## Phase 7: Automated Tests & Verification
- [ ] **7.1 Unit Tests (`LoanManagement.UnitTests`)**
  - [ ] Test EMI Amortization calculations for various principal amounts and tenors.
  - [ ] Test Repayment split allocation (Principal vs Interest vs Late Fees).
  - [ ] Test GL double-entry rule (Debit equals Credit validation).
  - [ ] Test Daily Interest Accrual formula.
- [ ] **7.2 Integration Tests (`LoanManagement.IntegrationTests`)**
  - [ ] Test idempotency behavior (submitting duplicate payment returns cached result).
  - [ ] Test complete loan lifecycle end-to-end: Create -> Approve -> Disburse -> Repay.

---

## Phase 8: Final Polish & Portfolio Readiness
- [ ] Update root `README.md` with Architecture Diagrams, System Overview, Setup Guide, and Swagger instructions.
- [ ] Verify clean build with zero warnings: `dotnet build`.
- [ ] Verify all tests pass: `dotnet test`.
