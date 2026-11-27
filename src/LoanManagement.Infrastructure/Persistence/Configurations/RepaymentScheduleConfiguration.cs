using LoanManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanManagement.Infrastructure.Persistence.Configurations;

public sealed class RepaymentScheduleConfiguration : IEntityTypeConfiguration<RepaymentScheduleItem>
{
    public void Configure(EntityTypeBuilder<RepaymentScheduleItem> builder)
    {
        builder.ToTable("repayment_schedules");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.LoanId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.PrincipalDue).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.InterestDue).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.TotalDue).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.RemainingPrincipal).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(100);

        builder.HasIndex(x => new { x.LoanId, x.Status, x.DueDate });
    }
}
