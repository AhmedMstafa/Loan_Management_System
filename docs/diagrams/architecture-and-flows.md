# Loan Management System — Diagrams & Architecture Flows

## 1. Clean Architecture System Overview

```mermaid
flowchart TB
    subgraph ClientLayer["Clients & Consumers"]
        WebPortal["Web Portal (Loan Officers / Underwriters / Ops / Customers)"]
        MobileApp["Mobile Banking App"]
        Scheduler["Background Scheduler (Quartz.NET / Cron)"]
    end

    subgraph APILayer["API Presentation Layer (ASP.NET Core)"]
        Controllers["Controllers / Endpoints"]
        Middlewares["Middlewares (Auth, Idempotency, Global Exception RFC 7807, Rate Limiting)"]
    end

    subgraph AppLayer["Application Layer (CQRS + MediatR)"]
        Commands["Commands & Handlers (CreateLoan, Disburse, Repay)"]
        Queries["Queries & Handlers (GetSchedule, GetDelinquent)"]
        Behaviors["Pipeline Behaviors (Validation, Logging, Performance, TransactionScope)"]
        Validators["FluentValidation Rules"]
    end

    subgraph DomainLayer["Domain Layer (Enterprise Core)"]
        Entities["Entities (Loan, Borrower, Repayment, LedgerEntry, AmortizationSchedule)"]
        Enums["Enums (LoanStatus, RiskRating, TransactionType)"]
        DomainEvents["Domain Events (LoanApprovedEvent, PaymentReceivedEvent)"]
        DomainServices["Financial Calculations (Amortization, Daily Accrual)"]
    end

    subgraph InfraLayer["Infrastructure Layer"]
        EFCore["EF Core DbContext & Migrations"]
        SqlDB[("Relational Database (SQL Server / PostgreSQL)")]
        CreditBureau["3rd-Party Credit Bureau Integration"]
        DocumentStore["Document Storage (S3 / Azure Blob / Local for KYC)"]
        AuditService["Audit Logger & Event Dispatcher"]
    end

    ClientLayer --> APILayer
    APILayer --> AppLayer
    AppLayer --> DomainLayer
    AppLayer --> InfraLayer
    InfraLayer --> SqlDB
    InfraLayer --> CreditBureau
    InfraLayer --> DocumentStore
```

---

## 2. Complete Loan State Lifecycle (State Machine)

```mermaid
stateDiagram-v2
    [*] --> Draft : Create Loan Application
    Draft --> Submitted : Submit with KYC Docs
    Submitted --> Underwriting : Run Credit Check
    Underwriting --> Approved : Underwriter Approves
    Underwriting --> Rejected : Underwriter Rejects
    Approved --> Disbursed : Back Office Disburses Funds
    Disbursed --> Active : Ledger & Schedule Initialized
    
    Active --> Active : Monthly Repayment Made
    Active --> Delinquent : Grace Period Expired (>30 Days Overdue)
    Delinquent --> Active : Arrears Paid In Full
    Delinquent --> Defaulted : Overdue >90/180 Days (NPL Provisioned)
    
    Active --> Closed : Paid In Full
    Active --> TopUpRequested : Apply For Top-Up
    TopUpRequested --> Active : Top-Up Approved & Added to Principal
    
    Rejected --> [*]
    Closed --> [*]
```

---

## 3. Financial Repayment & Double-Entry Ledger Flow

```mermaid
sequenceDiagram
    autonumber
    actor Customer as Customer / Collector
    participant API as Repayment Endpoint
    participant Idemp as Idempotency Filter
    participant MediatR as Command Handler
    participant Domain as Loan Aggregate
    participant Ledger as General Ledger
    participant DB as SQL Database

    Customer->>API: POST /api/v1/loans/{id}/repayments (Idempotency-Key)
    API->>Idemp: Check if key was previously processed
    alt Key already exists
        Idemp-->>Customer: Return Cached Previous Result (200 OK)
    else Key is new
        API->>MediatR: Send MakeRepaymentCommand
        MediatR->>Domain: Calculate split (Principal vs Interest vs Late Fees)
        Domain->>Domain: Update Outstanding Balance & Installment Status
        MediatR->>Ledger: Generate Debit (Cash/Bank) & Credit (Loan Receivable, Interest Income)
        MediatR->>DB: Save in Single Atomic Transaction (ACID)
        MediatR->>DB: Store Idempotency Record
        MediatR-->>API: Success DTO
        API-->>Customer: 200 OK (Payment Receipt & Updated Balance)
    end
```
