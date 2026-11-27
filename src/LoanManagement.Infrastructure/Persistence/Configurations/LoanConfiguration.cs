using LoanManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanManagement.Infrastructure.Persistence.Configurations;

public sealed class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("loans");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(50);
        builder.Property(x => x.OrganizationId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.BorrowerId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.ProductId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(x => x.RequestedAmount).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.ApprovedAmount).HasColumnType("decimal(18,4)");
        builder.Property(x => x.DisbursedAmount).HasColumnType("decimal(18,4)");
        builder.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        builder.Property(x => x.InterestRate).HasColumnType("decimal(8,4)").IsRequired();
        builder.Property(x => x.OutstandingPrincipal).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.AccruedInterest).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.ApprovedBy).HasMaxLength(50);
        builder.Property(x => x.Purpose).HasMaxLength(500);
        builder.Property(x => x.CollateralDescription).HasMaxLength(500);
        builder.Property(x => x.ConcurrencyToken).IsConcurrencyToken();
        builder.Property(x => x.CreatedBy).HasMaxLength(100);

        builder.HasIndex(x => new { x.OrganizationId, x.Status });
        builder.HasIndex(x => x.BorrowerId);

        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Borrower>().WithMany().HasForeignKey(x => x.BorrowerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<LoanProduct>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.RepaymentSchedule).WithOne().HasForeignKey(x => x.LoanId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Payments).WithOne().HasForeignKey(x => x.LoanId).OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.RepaymentSchedule).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(x => x.Payments).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
