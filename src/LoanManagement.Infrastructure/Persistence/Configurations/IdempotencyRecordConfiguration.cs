using LoanManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanManagement.Infrastructure.Persistence.Configurations;

public sealed class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.ToTable("idempotency_records");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(128);
        builder.Property(x => x.OrganizationId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.RequestHash).HasMaxLength(64).IsRequired();
        builder.Property(x => x.ResponsePayload).HasColumnType("nvarchar(max)").IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(100);

        builder.HasIndex(x => new { x.Id, x.OrganizationId });
        builder.HasIndex(x => x.ExpiresAt);
    }
}
