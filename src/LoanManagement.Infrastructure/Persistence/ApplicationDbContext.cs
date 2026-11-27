using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Borrower> Borrowers => Set<Borrower>();
    public DbSet<LoanProduct> LoanProducts => Set<LoanProduct>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<RepaymentScheduleItem> RepaymentScheduleItems => Set<RepaymentScheduleItem>();
    public DbSet<RepaymentPayment> RepaymentPayments => Set<RepaymentPayment>();
    public DbSet<GlAccount> GlAccounts => Set<GlAccount>();
    public DbSet<GlJournalEntry> GlJournalEntries => Set<GlJournalEntry>();
    public DbSet<GlJournalLine> GlJournalLines => Set<GlJournalLine>();
    public DbSet<KycDocument> KycDocuments => Set<KycDocument>();
    public DbSet<CreditCheck> CreditChecks => Set<CreditCheck>();
    public DbSet<Dispute> Disputes => Set<Dispute>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        Seed.SeedData.Apply(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
}
