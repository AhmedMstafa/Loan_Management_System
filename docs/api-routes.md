# Loan Management System — REST API Specification

This document refactors the 20 system business APIs into clean, standardized RESTful endpoints adhering to enterprise FinTech standards (RFC 7807 Error format, Idempotency headers, Role-based scopes, and decimal monetary precision).

---

## Global Headers & Standards
- `Authorization: Bearer <JWT_TOKEN>` (Must contain `org_id`, `sub`, and role claims).
- `Idempotency-Key: <UUID>` (Required for all mutating operations: POST, PUT, DELETE).
- `X-Correlation-ID: <UUID>` (For distributed tracing and logging).
- **Decimal Format**: All currencies must be serialized with 2 or 4 decimal places (e.g. `15000.00`).
- **Dates**: ISO-8601 UTC string (`YYYY-MM-DDTHH:mm:ssZ`).

---

## Endpoints Catalog

### API 01 — Create Loan Application
- **Method & Route**: `POST /api/v1/loans`
- **Role / Scope**: `LoanOfficer`, `Admin` (`loan_management.write`)
- **Headers**: `Idempotency-Key`
- **Request Body**:
```json
{
  "organizationId": "org_1",
  "borrowerId": "bor_9011",
  "productId": "prod_personal_01",
  "requestedAmount": 25000.00,
  "currency": "USD",
  "tenorMonths": 24,
  "annualInterestRate": 12.50,
  "purpose": "Home Renovation",
  "collateralDescription": "Vehicle Title"
}
```
- **Response**: `201 Created`
```json
{
  "id": "loa_1001",
  "status": "Draft",
  "organizationId": "org_1",
  "borrowerId": "bor_9011",
  "requestedAmount": 25000.00,
  "tenorMonths": 24,
  "createdAt": "2026-11-27T08:00:00Z"
}
```

---

### API 02 — Run Credit Check
- **Method & Route**: `POST /api/v1/loans/{id}/credit-check`
- **Role / Scope**: `Underwriter`, `Admin` (`loan_management.write`)
- **Request Body**:
```json
{
  "provider": "Equifax",
  "forceRefresh": false
}
```
- **Response**: `200 OK`
```json
{
  "loanId": "loa_1001",
  "creditScore": 745,
  "riskRating": "Low",
  "debtToIncomeRatio": 0.28,
  "recommendation": "Approve",
  "checkedAt": "2026-11-27T08:05:00Z"
}
```

---

### API 03 — Approve Loan
- **Method & Route**: `POST /api/v1/loans/{id}/approve`
- **Role / Scope**: `Underwriter`, `Admin` (`loan_management.write`)
- **Headers**: `Idempotency-Key`
- **Request Body**:
```json
{
  "approvedAmount": 25000.00,
  "approvedInterestRate": 12.50,
  "tenorMonths": 24,
  "notes": "Borrower passed underwriting criteria with high credit score.",
  "scheduledDisbursementDate": "2026-12-01"
}
```
- **Response**: `200 OK`
```json
{
  "loanId": "loa_1001",
  "status": "Approved",
  "approvedAmount": 25000.00,
  "approvedBy": "usr_underwriter_45",
  "approvedAt": "2026-11-27T08:10:00Z"
}
```

---

### API 04 — Disburse Loan
- **Method & Route**: `POST /api/v1/loans/{id}/disburse`
- **Role / Scope**: `BackOffice`, `Admin` (`loan_management.write`)
- **Headers**: `Idempotency-Key`
- **Request Body**:
```json
{
  "disbursedAmount": 25000.00,
  "disbursementMethod": "BankTransfer",
  "destinationAccount": {
    "bankCode": "CHASE",
    "accountNumber": "9876543210",
    "accountHolderName": "John Doe"
  },
  "firstRepaymentDueDate": "2027-01-01"
}
```
- **Response**: `200 OK`
```json
{
  "loanId": "loa_1001",
  "status": "Disbursed",
  "disbursementReference": "DISB_20261127_009",
  "disbursedAt": "2026-11-27T08:15:00Z",
  "firstDueDate": "2027-01-01"
}
```

---

### API 05 — View Payment Schedule (Amortization)
- **Method & Route**: `GET /api/v1/loans/{id}/payment-schedule`
- **Role / Scope**: `Customer`, `LoanOfficer`, `BackOffice`
- **Response**: `200 OK`
```json
{
  "loanId": "loa_1001",
  "totalPrincipal": 25000.00,
  "totalInterest": 3374.80,
  "currency": "USD",
  "installments": [
    {
      "installmentNumber": 1,
      "dueDate": "2027-01-01",
      "principal": 918.52,
      "interest": 260.42,
      "totalDue": 1178.94,
      "remainingBalance": 24081.48,
      "status": "Pending"
    }
  ]
}
```

---

### API 06 — Make Repayment
- **Method & Route**: `POST /api/v1/loans/{id}/repayments`
- **Role / Scope**: `Customer`, `Collector`, `BackOffice`
- **Headers**: `Idempotency-Key`
- **Request Body**:
```json
{
  "paymentAmount": 1178.94,
  "paymentMethod": "DirectDebit",
  "paymentReference": "PAY_TXN_881273",
  "paidAt": "2026-11-27T08:20:00Z"
}
```
- **Response**: `200 OK`
```json
{
  "paymentId": "pmt_4001",
  "loanId": "loa_1001",
  "amountPaid": 1178.94,
  "principalPortion": 918.52,
  "interestPortion": 260.42,
  "lateFeePortion": 0.00,
  "newOutstandingBalance": 24081.48,
  "processedAt": "2026-11-27T08:20:05Z"
}
```

---

### API 07 — List Delinquent Accounts (NPL / Overdue)
- **Method & Route**: `GET /api/v1/loans/delinquent`
- **Role / Scope**: `Collector`, `RiskAnalyst`, `Admin`
- **Query Params**: `minDaysOverdue=30&pageNumber=1&pageSize=20`
- **Response**: `200 OK`
```json
{
  "items": [
    {
      "loanId": "loa_0988",
      "borrowerName": "Jane Smith",
      "daysOverdue": 45,
      "overdueAmount": 2357.88,
      "outstandingBalance": 18500.00,
      "riskCategory": "Substandard",
      "lastPaymentDate": "2026-10-12"
    }
  ],
  "totalCount": 1,
  "pageNumber": 1,
  "pageSize": 20
}
```

---

### API 08 — Export Portfolio Report
- **Method & Route**: `POST /api/v1/reports/portfolio-export`
- **Role / Scope**: `RiskAnalyst`, `Admin`
- **Request Body**:
```json
{
  "format": "CSV",
  "asOfDate": "2026-11-27",
  "filterStatus": ["Active", "Delinquent", "Defaulted"]
}
```
- **Response**: `200 OK`
```json
{
  "reportId": "rep_9912",
  "downloadUrl": "https://storage.bank.internal/reports/portfolio_20261127.csv",
  "totalLoans": 1420,
  "totalOutstandingPrincipal": 34500000.00,
  "generatedAt": "2026-11-27T08:25:00Z"
}
```

---

### API 09 — Upload KYC Documents
- **Method & Route**: `POST /api/v1/borrowers/{id}/kyc-documents`
- **Role / Scope**: `Compliance`, `LoanOfficer`
- **Content-Type**: `multipart/form-data`
- **Form Fields**: `documentType` (e.g. `NationalId`, `ProofOfIncome`), `file` (binary)
- **Response**: `201 Created`
```json
{
  "documentId": "doc_5521",
  "borrowerId": "bor_9011",
  "documentType": "NationalId",
  "verificationStatus": "PendingReview",
  "uploadedAt": "2026-11-27T08:30:00Z"
}
```

---

### API 10 — Generate Daily Interest Accruals
- **Method & Route**: `POST /api/v1/batch/interest-accruals`
- **Role / Scope**: `System`, `Admin`
- **Request Body**:
```json
{
  "accrualDate": "2026-11-27"
}
```
- **Response**: `200 OK`
```json
{
  "processedLoansCount": 1350,
  "totalAccruedInterest": 45120.35,
  "batchRunId": "batch_accrual_20261127",
  "completedAt": "2026-11-27T08:35:00Z"
}
```

---

### API 11 — Upload Bank Reconciliations
- **Method & Route**: `POST /api/v1/accounting/bank-reconciliations`
- **Role / Scope**: `Ops`, `Accountant`
- **Request Body**:
```json
{
  "bankStatementDate": "2026-11-26",
  "transactions": [
    {
      "bankTxnId": "BTXN_9918",
      "amount": 1178.94,
      "reference": "PAY_TXN_881273",
      "date": "2026-11-26"
    }
  ]
}
```
- **Response**: `200 OK`
```json
{
  "totalProcessed": 1,
  "matchedCount": 1,
  "unmatchedCount": 0,
  "status": "Reconciled"
}
```

---

### API 12 — Request Loan Statement
- **Method & Route**: `GET /api/v1/loans/{id}/statement`
- **Role / Scope**: `Customer`, `LoanOfficer`
- **Query Params**: `from=2026-01-01&to=2026-11-27&format=PDF`
- **Response**: `200 OK`
```json
{
  "loanId": "loa_1001",
  "statementUrl": "https://storage.bank.internal/statements/loa_1001_20261127.pdf",
  "openingBalance": 25000.00,
  "closingBalance": 24081.48,
  "totalInterestPaid": 260.42,
  "totalPrincipalPaid": 918.52
}
```

---

### API 13 — Manage Product Rates & Fees
- **Method & Route**: `PUT /api/v1/products/{id}/rates`
- **Role / Scope**: `Admin`
- **Request Body**:
```json
{
  "baseInterestRate": 13.00,
  "minLoanAmount": 1000.00,
  "maxLoanAmount": 50000.00,
  "originationFeePercentage": 1.50,
  "lateFeeFlatRate": 25.00,
  "effectiveDate": "2026-12-01"
}
```
- **Response**: `200 OK`
```json
{
  "productId": "prod_personal_01",
  "status": "Updated",
  "effectiveDate": "2026-12-01",
  "updatedAt": "2026-11-27T08:40:00Z"
}
```

---

### API 14 — Trigger Provisioning Charge (NPL Provision)
- **Method & Route**: `POST /api/v1/risk/provisioning-charges`
- **Role / Scope**: `RiskAnalyst`, `Admin`
- **Request Body**:
```json
{
  "period": "2026-Q4",
  "provisioningModel": "IFRS-9-ECL"
}
```
- **Response**: `200 OK`
```json
{
  "period": "2026-Q4",
  "totalImpairedLoans": 45,
  "totalProvisionCharge": 182500.00,
  "postedGlJournalEntryId": "GL_ENT_9921",
  "processedAt": "2026-11-27T08:45:00Z"
}
```

---

### API 15 — Post GL Journal Entries
- **Method & Route**: `POST /api/v1/accounting/gl-journal-entries`
- **Role / Scope**: `Accountant`, `Admin`
- **Headers**: `Idempotency-Key`
- **Request Body**:
```json
{
  "accountingDate": "2026-11-27",
  "memo": "Monthly loan servicing adjustment",
  "lines": [
    {
      "accountNumber": "1100-Cash",
      "debit": 1178.94,
      "credit": 0.00
    },
    {
      "accountNumber": "1200-LoanPrincipalReceivable",
      "debit": 0.00,
      "credit": 918.52
    },
    {
      "accountNumber": "4100-InterestIncome",
      "debit": 0.00,
      "credit": 260.42
    }
  ]
}
```
- **Response**: `201 Created`
```json
{
  "journalEntryId": "GL_ENT_9922",
  "status": "Posted",
  "totalDebit": 1178.94,
  "totalCredit": 1178.94,
  "postedAt": "2026-11-27T08:50:00Z"
}
```

---

### API 16 — Bulk Import Loan Data
- **Method & Route**: `POST /api/v1/loans/bulk-import`
- **Role / Scope**: `Admin`
- **Content-Type**: `multipart/form-data`
- **Response**: `202 Accepted`
```json
{
  "importJobId": "job_imp_4412",
  "status": "Queued",
  "statusCheckUrl": "/api/v1/jobs/imp_4412",
  "queuedAt": "2026-11-27T08:55:00Z"
}
```

---

### API 17 — Apply For Top-Up
- **Method & Route**: `POST /api/v1/loans/{id}/top-up`
- **Role / Scope**: `Customer`, `LoanOfficer`
- **Headers**: `Idempotency-Key`
- **Request Body**:
```json
{
  "additionalAmount": 5000.00,
  "tenorExtensionMonths": 6,
  "reason": "Business expansion"
}
```
- **Response**: `200 OK`
```json
{
  "topUpRequestId": "top_7712",
  "loanId": "loa_1001",
  "status": "Submitted",
  "newTotalPrincipal": 29081.48,
  "submittedAt": "2026-11-27T09:00:00Z"
}
```

---

### API 18 — Resolve Customer Dispute
- **Method & Route**: `POST /api/v1/loans/{id}/disputes/resolve`
- **Role / Scope**: `Support`, `Admin`
- **Request Body**:
```json
{
  "disputeId": "disp_3311",
  "resolutionType": "WaiveLateFee",
  "adjustmentAmount": 50.00,
  "resolutionNotes": "System billing error during maintenance; fee waived."
}
```
- **Response**: `200 OK`
```json
{
  "disputeId": "disp_3311",
  "status": "Resolved",
  "adjustedAmount": 50.00,
  "resolvedBy": "usr_support_21",
  "resolvedAt": "2026-11-27T09:05:00Z"
}
```

---

### API 19 — Run Batch Billing (Statement Generation)
- **Method & Route**: `POST /api/v1/batch/billing`
- **Role / Scope**: `Scheduler`, `Admin`
- **Request Body**:
```json
{
  "billingCycleDate": "2026-11-30"
}
```
- **Response**: `200 OK`
```json
{
  "cycleDate": "2026-11-30",
  "statementsGenerated": 1280,
  "invoicesDispatched": 1280,
  "status": "Completed"
}
```

---

### API 20 — Audit Access Logs
- **Method & Route**: `GET /api/v1/audit/logs`
- **Role / Scope**: `Security`, `Compliance`, `Admin`
- **Query Params**: `entity=Loan&entityId=loa_1001&pageNumber=1&pageSize=50`
- **Response**: `200 OK`
```json
{
  "items": [
    {
      "id": "aud_8819",
      "timestamp": "2026-11-27T08:15:00Z",
      "action": "LoanDisbursed",
      "actorId": "usr_backoffice_10",
      "actorRole": "BackOffice",
      "entity": "Loan",
      "entityId": "loa_1001",
      "ipAddress": "192.168.1.100",
      "changes": {
        "previousStatus": "Approved",
        "newStatus": "Disbursed"
      }
    }
  ],
  "totalCount": 1,
  "pageNumber": 1,
  "pageSize": 50
}
```

---

## Standard Error Response (RFC 7807)
```json
{
  "type": "https://api.loanmanagementsystem.com/errors/conflict",
  "title": "Idempotency Conflict",
  "status": 409,
  "detail": "An operation with this Idempotency-Key is currently being processed or already committed with different payload.",
  "instance": "/api/v1/loans/loa_1001/disburse",
  "errors": {
    "Idempotency-Key": ["Duplicate key submitted with differing hash"]
  }
}
```
