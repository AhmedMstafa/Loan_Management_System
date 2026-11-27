using LoanManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanManagement.Infrastructure.Persistence.Configurations;

public sealed class DisputeConfiguration : IEntityTypeConfiguration<Dispute>
{
    public void Configure(EntityTypeBuilder<Dispute> builder)
    {
        builder.ToTable("disputes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(50);
        builder.Property(x => x.OrganizationId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.LoanId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Reason).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(x => x.ResolvedBy).HasMaxLength(100);
        builder.Property(x => x.ResolutionType).HasMaxLength(100);
        builder.Property(x => x.AdjustmentAmount).HasColumnType("decimal(18,4)");
        builder.Property(x => x.ResolutionNotes).HasMaxLength(2000);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);

        builder.HasIndex(x => new { x.OrganizationId, x.LoanId, x.Status });
        builder.HasOne<Loan>().WithMany().HasForeignKey(x => x.LoanId).OnDelete(DeleteBehavior.Cascade);
    }
}
