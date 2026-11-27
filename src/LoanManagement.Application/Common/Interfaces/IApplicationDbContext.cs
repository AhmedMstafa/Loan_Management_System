using LoanManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Organization> Organizations { get; }
    DbSet<Borrower> Borrowers { get; }
    DbSet<LoanProduct> LoanProducts { get; }
    DbSet<Loan> Loans { get; }
    DbSet<RepaymentScheduleItem> RepaymentScheduleItems { get; }
    DbSet<RepaymentPayment> RepaymentPayments { get; }
    DbSet<GlAccount> GlAccounts { get; }
    DbSet<GlJournalEntry> GlJournalEntries { get; }
    DbSet<GlJournalLine> GlJournalLines { get; }
    DbSet<KycDocument> KycDocuments { get; }
    DbSet<CreditCheck> CreditChecks { get; }
    DbSet<Dispute> Disputes { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<IdempotencyRecord> IdempotencyRecords { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
