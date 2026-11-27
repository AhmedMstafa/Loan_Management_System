using LoanManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanManagement.Infrastructure.Persistence.Configurations;

public sealed class LoanProductConfiguration : IEntityTypeConfiguration<LoanProduct>
{
    public void Configure(EntityTypeBuilder<LoanProduct> builder)
    {
        builder.ToTable("loan_products");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(50);
        builder.Property(x => x.OrganizationId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.BaseInterestRate).HasColumnType("decimal(8,4)").IsRequired();
        builder.Property(x => x.MinAmount).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.MaxAmount).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.OriginationFeePct).HasColumnType("decimal(5,2)").IsRequired();
        builder.Property(x => x.LateFeeFlat).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(100);

        builder.HasIndex(x => new { x.OrganizationId, x.IsActive });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}
