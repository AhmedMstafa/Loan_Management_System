using LoanManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanManagement.Infrastructure.Persistence.Configurations;

public sealed class GlJournalLineConfiguration : IEntityTypeConfiguration<GlJournalLine>
{
    public void Configure(EntityTypeBuilder<GlJournalLine> builder)
    {
        builder.ToTable("gl_journal_lines");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.JournalEntryId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.AccountNumber).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Debit).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.Credit).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(100);

        builder.HasIndex(x => x.JournalEntryId);
        builder.HasOne<GlAccount>().WithMany().HasForeignKey(x => x.AccountNumber).OnDelete(DeleteBehavior.Restrict);
    }
}
