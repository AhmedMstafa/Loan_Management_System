using LoanManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanManagement.Infrastructure.Persistence.Configurations;

public sealed class RepaymentPaymentConfiguration : IEntityTypeConfiguration<RepaymentPayment>
{
    public void Configure(EntityTypeBuilder<RepaymentPayment> builder)
    {
        builder.ToTable("repayment_transactions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(50);
        builder.Property(x => x.LoanId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.OrganizationId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.AmountPaid).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.PrincipalAllocated).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.InterestAllocated).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.LateFeeAllocated).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.PaymentMethod).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(x => x.TransactionReference).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(100);

        builder.HasIndex(x => x.TransactionReference).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.LoanId });
    }
}
