# Loan Management System — Backend Architecture Structure

## 1. Solution Architecture & Dependency Rules

We follow **Clean Architecture** with strict inward dependency rules:

```
                  ┌──────────────────────┐
                  │      Domain          │ (Pure C#, No External Dependencies)
                  └──────────▲───────────┘
                             │
                  ┌──────────┴───────────┐
                  │    Application       │ (MediatR CQRS, FluentValidation, DTOs)
                  └─────▲──────────▲─────┘
                        │          │
         ┌──────────────┴───┐  ┌───┴──────────────┐
         │  Infrastructure  │  │   API / Web      │ (Controllers, Middlewares, Host)
         └──────────────────┘  └──────────────────┘
```

- **Domain**: Cannot reference any other project or NuGet package (except domain primitive helpers).
- **Application**: References `Domain` only.
- **Infrastructure**: References `Application` (implements its interfaces: EF Core, Repositories, 3rd-party clients).
- **API**: References `Application` and `Infrastructure` for Dependency Injection composition root.

---

## 2. Directory & Namespace Structure

```text
Loan_Management_System/
├── src/
│   ├── LoanManagement.Domain/
│   │   ├── Common/
│   │   │   ├── BaseEntity.cs                 # Id, CreatedAt, CreatedBy, UpdatedAt, IsDeleted
│   │   │   ├── IAggregateRoot.cs
│   │   │   ├── IDomainEvent.cs
│   │   │   └── ValueObject.cs
│   │   ├── Entities/
│   │   │   ├── Loan.cs                       # Aggregate Root for loan operations
│   │   │   ├── Borrower.cs                   # Borrower identity, KYC, contact details
│   │   │   ├── LoanProduct.cs                # Product terms, rates, limits
│   │   │   ├── RepaymentScheduleItem.cs      # Amortization installments (principal, interest, status)
│   │   │   ├── RepaymentPayment.cs           # Recorded payment allocations
│   │   │   ├── GlJournalEntry.cs             # Double-entry ledger header
│   │   │   ├── GlJournalLine.cs              # Debit/Credit lines
│   │   │   ├── KycDocument.cs                # Attached verification documents
│   │   │   ├── Dispute.cs                    # Customer disputes and resolutions
│   │   │   └── AuditLog.cs                   # Immutable security & audit trail
│   │   ├── Enums/
│   │   │   ├── LoanStatus.cs                 # Draft, Submitted, Underwriting, Approved, Disbursed, Active, Delinquent, Closed
│   │   │   ├── RepaymentStatus.cs            # Pending, Paid, PartiallyPaid, Overdue
│   │   │   ├── RiskRating.cs                 # Low, Medium, High, Defaulted
│   │   │   └── TransactionType.cs            # Disbursement, PrincipalPayment, InterestPayment, Fee, Provision
│   │   ├── Events/
│   │   │   ├── LoanApprovedDomainEvent.cs
│   │   │   ├── LoanDisbursedDomainEvent.cs
│   │   │   └── PaymentReceivedDomainEvent.cs
│   │   └── Exceptions/
│   │       ├── DomainValidationException.cs
│   │       └── InvalidLoanStateException.cs
│   │
│   ├── LoanManagement.Application/
│   │   ├── Common/
│   │   │   ├── Behaviors/
│   │   │   │   ├── ValidationBehavior.cs     # FluentValidation MediatR pipeline
│   │   │   │   ├── LoggingBehavior.cs        # Request/Response duration & correlation
│   │   │   │   └── TransactionBehavior.cs    # Atomic DB transaction per command
│   │   │   ├── Interfaces/
│   │   │   │   ├── IApplicationDbContext.cs  # DbSets abstraction
│   │   │   │   ├── ICreditBureauService.cs   # External credit check interface
│   │   │   │   ├── ICurrentUserService.cs    # UserId, OrgId, Roles from JWT
│   │   │   │   ├── IDateTimeProvider.cs
│   │   │   │   └── IIdempotencyService.cs
│   │   │   └── Models/
│   │   │       ├── PaginatedList.cs
│   │   │       └── Result.cs
│   │   └── Features/
│   │       ├── Loans/
│   │       │   ├── Commands/
│   │       │   │   ├── CreateLoanApplication/
│   │       │   │   │   ├── CreateLoanCommand.cs
│   │       │   │   │   ├── CreateLoanCommandHandler.cs
│   │       │   │   │   └── CreateLoanCommandValidator.cs
│   │       │   │   ├── ApproveLoan/
│   │       │   │   ├── DisburseLoan/
│   │       │   │   ├── ApplyTopUp/
│   │       │   │   └── RunCreditCheck/
│   │       │   └── Queries/
│   │       │       ├── GetPaymentSchedule/
│   │       │       ├── GetDelinquentLoans/
│   │       │       └── GetLoanStatement/
│   │       ├── Repayments/
│   │       │   └── Commands/MakeRepayment/
│   │       ├── Accounting/
│   │       │   ├── Commands/PostGlJournalEntry/
│   │       │   └── Commands/ReconcileBankStatement/
│   │       ├── Batch/
│   │       │   ├── Commands/GenerateDailyAccruals/
│   │       │   └── Commands/RunBatchBilling/
│   │       ├── Risk/
│   │       │   ├── Commands/TriggerProvisioningCharge/
│   │       │   └── Queries/ExportPortfolioReport/
│   │       └── Audit/
│   │           └── Queries/GetAuditLogs/
│   │
│   ├── LoanManagement.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   ├── Configurations/              # EF Core IEntityTypeConfiguration<T>
│   │   │   │   ├── LoanConfiguration.cs
│   │   │   │   ├── RepaymentScheduleConfiguration.cs
│   │   │   │   └── GlJournalLineConfiguration.cs
│   │   │   ├── Interceptors/
│   │   │   │   ├── AuditableEntityInterceptor.cs # Sets CreatedAt, OrgId, UpdatedAt
│   │   │   │   └── DispatchDomainEventsInterceptor.cs
│   │   │   └── Migrations/
│   │   ├── Services/
│   │   │   ├── CreditBureauService.cs       # Mock / 3rd-party HTTP client
│   │   │   ├── IdempotencyService.cs        # Redis or SQL backed idempotency store
│   │   │   └── CurrentUserService.cs        # IHttpContextAccessor wrapper
│   │   └── BackgroundJobs/
│   │       ├── DailyInterestAccrualJob.cs   # Quartz.NET / BackgroundService (runs daily at 00:00 UTC)
│   │       └── MonthlyBillingJob.cs         # Runs at end of month
│   │
│   └── LoanManagement.API/
│       ├── Controllers/
│       │   ├── BaseApiController.cs
│       │   ├── LoansController.cs
│       │   ├── RepaymentsController.cs
│       │   ├── AccountingController.cs
│       │   ├── BatchController.cs
│       │   ├── RiskController.cs
│       │   └── AuditController.cs
│       ├── Middlewares/
│       │   ├── ExceptionHandlingMiddleware.cs # RFC 7807 ProblemDetails
│       │   └── IdempotencyMiddleware.cs     # Enforces Idempotency-Key header
│       ├── Filters/
│       │   └── TenantAuthorizationFilter.cs # Enforces organizationId match
│       ├── Program.cs
│       └── appsettings.json
│
└── tests/
    ├── LoanManagement.UnitTests/            # Domain tests: Amortization & Accrual formulas
    └── LoanManagement.IntegrationTests/     # WebApplicationFactory + Database tests
```

---

## 3. Financial Computation Logic (Domain Services)

### A. Monthly Amortization Schedule Calculation (Equal Monthly Installments - EMI)
For a loan with Principal $P$, Monthly Interest Rate $r = \frac{\text{Annual Rate}}{12 \times 100}$, and number of months $n$:
$$\text{EMI} = \frac{P \times r \times (1 + r)^n}{(1 + r)^n - 1}$$
- Each month:
  - $\text{Interest}_k = \text{Remaining Principal}_{k-1} \times r$
  - $\text{Principal}_k = \text{EMI} - \text{Interest}_k$
  - $\text{Remaining Principal}_k = \text{Remaining Principal}_{k-1} - \text{Principal}_k$
- Must use `decimal` with `MidpointRounding.ToEven` for exact penny balancing on final installment.

### B. Daily Accrual Formula
$$\text{Daily Accrual} = \frac{\text{Outstanding Balance} \times \text{Annual Rate}}{365}$$
Processed nightly via `DailyInterestAccrualJob`.

---

## 4. Key Cross-Cutting Concerns
1. **Idempotency Strategy**:
   - Cache `Idempotency-Key` + Request SHA256 Hash + Response status & body.
   - TTL: 24 to 48 hours.
   - If incoming request has same key and same hash: return cached response immediately.
   - If incoming request has same key with different hash: return `409 Conflict`.
2. **Audit Logging**:
   - `AuditableEntityInterceptor` automatically records previous state vs new state in JSON into `AuditLogs` table for any entity inheriting from `BaseEntity`.
3. **Global Error Handling**:
   - Intercepts `DomainValidationException` -> Returns `400 Bad Request` with field details.
   - Intercepts `NotFoundException` -> Returns `404 Not Found`.
   - Intercepts concurrency conflicts -> Returns `409 Conflict`.
