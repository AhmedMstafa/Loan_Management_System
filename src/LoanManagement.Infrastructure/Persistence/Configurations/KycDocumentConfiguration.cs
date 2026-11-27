using LoanManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanManagement.Infrastructure.Persistence.Configurations;

public sealed class KycDocumentConfiguration : IEntityTypeConfiguration<KycDocument>
{
    public void Configure(EntityTypeBuilder<KycDocument> builder)
    {
        builder.ToTable("kyc_documents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(50);
        builder.Property(x => x.OrganizationId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.BorrowerId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.DocumentType).HasMaxLength(100).IsRequired();
        builder.Property(x => x.StoragePath).HasMaxLength(500).IsRequired();
        builder.Property(x => x.VerificationStatus).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(x => x.VerifiedBy).HasMaxLength(100);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);

        builder.HasIndex(x => new { x.OrganizationId, x.BorrowerId });
        builder.HasOne<Borrower>().WithMany().HasForeignKey(x => x.BorrowerId).OnDelete(DeleteBehavior.Restrict);
    }
}
