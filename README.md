# Loan Management System (FinTech Enterprise Backend)

A comprehensive, production-grade **Loan Management System** built with **.NET 10 / ASP.NET Core**, adhering to **Clean Architecture**, **CQRS (MediatR)**, and strict financial domain practices (Idempotency, Double-Entry General Ledger, IFRS 9 ECL Provisioning, Audit Vault, Multi-tenancy).

---

## 📁 Project Documentation & AI Coding Kit

All architectural plans, specifications, and checklists are located in `docs/` and ready for execution by AI coding agents (OpenCode, Claude Code, Cursor, Codex):

- **[AGENTS.md](file:///c:/Users/windows10/source/repos/Loan_Management_System/AGENTS.md)**: Master AI agent operating rules, financial integrity constraints (decimal precision, ACID, Idempotency), and workflow triggers.
- **[docs/todo.md](file:///c:/Users/windows10/source/repos/Loan_Management_System/docs/todo.md)**: Step-by-step implementation checklist organized by phases.
- **[docs/api-routes.md](file:///c:/Users/windows10/source/repos/Loan_Management_System/docs/api-routes.md)**: Full RESTful specification for all 20 business APIs with schemas, status codes, and idempotency headers.
- **[docs/backend-structure.md](file:///c:/Users/windows10/source/repos/Loan_Management_System/docs/backend-structure.md)**: Clean Architecture layer division, MediatR CQRS handlers, EF Core configuration, and financial formulas (Amortization & Accruals).
- **[docs/db-schema.md](file:///c:/Users/windows10/source/repos/Loan_Management_System/docs/db-schema.md)**: Relational schema design with exact SQL data types (`DECIMAL(18,4)`), foreign keys, indexes, and double-entry ledger design.
- **[docs/pages-map.md](file:///c:/Users/windows10/source/repos/Loan_Management_System/docs/pages-map.md)**: UI views, user roles (Loan Officer, Underwriter, Ops, Collector, Customer), and API mapping.
- **[docs/diagrams/architecture-and-flows.md](file:///c:/Users/windows10/source/repos/Loan_Management_System/docs/diagrams/architecture-and-flows.md)**: Visual architecture flowcharts, loan state machine, and sequence diagrams.

---

## 🚀 Getting Started

```bash
# Build the solution
dotnet build

# Run tests
dotnet test

# Run API
dotnet run --project Loan_Management_System
```
