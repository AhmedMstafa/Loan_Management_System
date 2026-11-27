using LoanManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanManagement.Infrastructure.Persistence.Configurations;

public sealed class GlAccountConfiguration : IEntityTypeConfiguration<GlAccount>
{
    public void Configure(EntityTypeBuilder<GlAccount> builder)
    {
        builder.ToTable("gl_accounts");
        builder.HasKey(x => x.AccountNumber);
        builder.Property(x => x.AccountNumber).HasMaxLength(50);
        builder.Property(x => x.AccountName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Category).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
    }
}
