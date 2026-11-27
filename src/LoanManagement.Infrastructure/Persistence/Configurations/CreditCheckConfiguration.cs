using LoanManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanManagement.Infrastructure.Persistence.Configurations;

public sealed class CreditCheckConfiguration : IEntityTypeConfiguration<CreditCheck>
{
    public void Configure(EntityTypeBuilder<CreditCheck> builder)
    {
        builder.ToTable("credit_checks");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(50);
        builder.Property(x => x.OrganizationId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.LoanId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Provider).HasMaxLength(100).IsRequired();
        builder.Property(x => x.RiskRating).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(x => x.DebtToIncomeRatio).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.Recommendation).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(100);

        builder.HasIndex(x => new { x.LoanId, x.CheckedAt });
        builder.HasOne<Loan>().WithMany().HasForeignKey(x => x.LoanId).OnDelete(DeleteBehavior.Cascade);
    }
}
