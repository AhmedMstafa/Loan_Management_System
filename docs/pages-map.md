# Loan Management System — Pages & Roles Map

This document maps out the system's user interfaces, portal views, user personas, and the corresponding backend APIs that power each screen.

---

## 1. Portals & User Roles

| Portal | Intended Persona | Key Responsibilities |
|---|---|---|
| **Loan Origination & Servicing Portal** | Loan Officer | Apply for loans, upload borrower KYC, track pipeline |
| **Credit & Underwriting Dashboard** | Underwriter | Review credit checks, approve/reject loans, set terms |
| **Back Office & Operations Panel** | Ops / Back Office | Disburse funds, reconcile bank statements, manage billing |
| **Collections & Risk Control Desk** | Collector / Risk Analyst | Delinquent accounts monitoring, IFRS 9 provisioning charges, portfolio risk export |
| **Finance & General Ledger Console**| Accountant / CFO | GL journal entries, reconciliation reports, interest accrual logs |
| **Customer Self-Service Portal** | Borrower (Customer) | View schedule, make repayments, request top-up, download statements |
| **System Administration & Security**| Admin / Compliance | Product rate management, dispute resolution, audit trail inspection |

---

## 2. Pages Breakdown & API Mapping

### A. Customer Self-Service Portal (`/portal/customer`)
1. **Loan Overview & Dashboard (`/loans/{id}`)**:
   - Displays current outstanding balance, next due date, loan status.
   - **API**: `GET /api/v1/loans/{id}/payment-schedule` (API 05)
2. **Repayment Screen (`/loans/{id}/pay`)**:
   - Pay installment or custom amount via card or bank debit.
   - **API**: `POST /api/v1/loans/{id}/repayments` (API 06)
3. **Statements & History (`/loans/{id}/statement`)**:
   - Filter by date range and download official PDF statements.
   - **API**: `GET /api/v1/loans/{id}/statement` (API 12)
4. **Top-Up Request (`/loans/{id}/top-up`)**:
   - Form to request additional borrowing amount.
   - **API**: `POST /api/v1/loans/{id}/top-up` (API 17)

---

### B. Loan Origination Portal (`/portal/origination`)
1. **New Loan Application (`/loans/new`)**:
   - Form to select product, input amount, tenor, and borrower details.
   - **API**: `POST /api/v1/loans` (API 01)
2. **KYC Verification Desk (`/borrowers/{id}/kyc`)**:
   - Upload national ID, proof of address, income slips.
   - **API**: `POST /api/v1/borrowers/{id}/kyc-documents` (API 09)
3. **Application Tracker (`/loans/pipeline`)**:
   - Real-time pipeline status (Draft, Underwriting, Approved).
   - **API**: `GET /api/v1/loans`

---

### C. Underwriting & Risk Desk (`/portal/underwriting`)
1. **Credit Check & Scorecard (`/underwriting/loans/{id}/credit`)**:
   - Trigger bureau inquiry, display debt-to-income (DTI) and risk score.
   - **API**: `POST /api/v1/loans/{id}/credit-check` (API 02)
2. **Approval Workspace (`/underwriting/loans/{id}/decision`)**:
   - Confirm final rate, sanctioned amount, disbursement schedule, notes.
   - **API**: `POST /api/v1/loans/{id}/approve` (API 03)
3. **Portfolio Risk & Delinquency Monitor (`/risk/delinquents`)**:
   - Table of overdue loans (30+, 60+, 90+ days), risk grades, contact actions.
   - **API**: `GET /api/v1/loans/delinquent` (API 07)
4. **Provisioning & ECL Calculator (`/risk/provisions`)**:
   - IFRS 9 ECL calculation trigger and impairment reports.
   - **API**: `POST /api/v1/risk/provisioning-charges` (API 14)
5. **Portfolio Risk Export (`/risk/export`)**:
   - Generate full portfolio snapshot and download as CSV/Excel.
   - **API**: `POST /api/v1/reports/portfolio-export` (API 08)

---

### D. Operations & Accounting Console (`/portal/ops`)
1. **Disbursement Queue (`/ops/disbursements`)**:
   - List approved loans awaiting payout; initiate bank transfer.
   - **API**: `POST /api/v1/loans/{id}/disburse` (API 04)
2. **Bank Reconciliation (`/ops/reconciliations`)**:
   - Upload MT940 / CSV bank feed and auto-match against repayments.
   - **API**: `POST /api/v1/accounting/bank-reconciliations` (API 11)
3. **General Ledger Journal Postings (`/accounting/gl-entries`)**:
   - Manual & automated double-entry postings (Debit/Credit).
   - **API**: `POST /api/v1/accounting/gl-journal-entries` (API 15)
4. **Dispute Resolution Desk (`/support/disputes`)**:
   - Manage complaints, fee adjustments, and payment adjustments.
   - **API**: `POST /api/v1/loans/{id}/disputes/resolve` (API 18)

---

### E. System Operations & Admin (`/portal/admin`)
1. **Product & Rate Configurator (`/admin/products`)**:
   - Set interest rates, fees, min/max limits.
   - **API**: `PUT /api/v1/products/{id}/rates` (API 13)
2. **Batch Jobs Control Panel (`/admin/jobs`)**:
   - Run or monitor daily accruals and monthly billing runs.
   - **APIs**:
     - `POST /api/v1/batch/interest-accruals` (API 10)
     - `POST /api/v1/batch/billing` (API 19)
3. **Legacy Migration & Bulk Import (`/admin/import`)**:
   - Bulk upload legacy portfolio records.
   - **API**: `POST /api/v1/loans/bulk-import` (API 16)
4. **Security & Audit Vault (`/admin/audit`)**:
   - Search access logs by user, date, entity, and action.
   - **API**: `GET /api/v1/audit/logs` (API 20)
