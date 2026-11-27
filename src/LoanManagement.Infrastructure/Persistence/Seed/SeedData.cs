using LoanManagement.Domain.Entities;
using LoanManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Infrastructure.Persistence.Seed;

public static class SeedData
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        var seedDate = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        modelBuilder.Entity<Organization>().HasData(new
        {
            Id = "org_default",
            Name = "Default Organization",
            IsActive = true,
            CreatedAt = seedDate,
            CreatedBy = (string?)"system",
            UpdatedAt = (DateTimeOffset?)null,
            IsDeleted = false
        });

        modelBuilder.Entity<LoanProduct>().HasData(
            new
            {
                Id = "prod_personal_01",
                OrganizationId = "org_default",
                Name = "Personal Loan",
                BaseInterestRate = 12.5000m,
                MinAmount = 1000.0000m,
                MaxAmount = 50000.0000m,
                MinTenorMonths = 6,
                MaxTenorMonths = 60,
                OriginationFeePct = 1.50m,
                LateFeeFlat = 25.0000m,
                IsActive = true,
                CreatedAt = seedDate,
                CreatedBy = (string?)"system",
                UpdatedAt = (DateTimeOffset?)null,
                IsDeleted = false
            },
            new
            {
                Id = "prod_business_01",
                OrganizationId = "org_default",
                Name = "Business Loan",
                BaseInterestRate = 15.0000m,
                MinAmount = 5000.0000m,
                MaxAmount = 250000.0000m,
                MinTenorMonths = 12,
                MaxTenorMonths = 120,
                OriginationFeePct = 2.00m,
                LateFeeFlat = 50.0000m,
                IsActive = true,
                CreatedAt = seedDate,
                CreatedBy = (string?)"system",
                UpdatedAt = (DateTimeOffset?)null,
                IsDeleted = false
            });

        modelBuilder.Entity<GlAccount>().HasData(
            new { AccountNumber = "1100-Cash", AccountName = "Cash", Category = GlAccountCategory.Asset, IsActive = true },
            new { AccountNumber = "1200-LoansReceivable", AccountName = "Loans Receivable", Category = GlAccountCategory.Asset, IsActive = true },
            new { AccountNumber = "1210-InterestReceivable", AccountName = "Interest Receivable", Category = GlAccountCategory.Asset, IsActive = true },
            new { AccountNumber = "2100-DisbursementPayable", AccountName = "Disbursement Payable", Category = GlAccountCategory.Liability, IsActive = true },
            new { AccountNumber = "4100-InterestIncome", AccountName = "Interest Income", Category = GlAccountCategory.Revenue, IsActive = true },
            new { AccountNumber = "4200-FeeIncome", AccountName = "Fee Income", Category = GlAccountCategory.Revenue, IsActive = true },
            new { AccountNumber = "5100-ProvisionExpense", AccountName = "Provision Expense", Category = GlAccountCategory.Expense, IsActive = true }
        );

        modelBuilder.Entity<User>().HasData(
            new
            {
                Id = "usr_admin_01",
                OrganizationId = "org_default",
                Email = "admin@bank.com",
                PasswordHash = "v1.100000.7ofzdEV3XgcM58cJqhvKrw==.nNfPleuFQ85eYAxJ5MYXQHtivS4L7yKbPXpHAkQyNPs=",
                FullName = "System Admin",
                Role = "Admin",
                IsActive = true,
                CreatedAt = seedDate,
                CreatedBy = (string?)"system",
                UpdatedAt = (DateTimeOffset?)null,
                IsDeleted = false
            },
            new
            {
                Id = "usr_officer_01",
                OrganizationId = "org_default",
                Email = "officer@bank.com",
                PasswordHash = "v1.100000.MDWj9SbEjHU6tQB0DuZLGg==.duINfkxwhUCpFIvrzN1aqajQgDev0pIE7J3PZkYMAdc=",
                FullName = "Loan Officer",
                Role = "LoanOfficer",
                IsActive = true,
                CreatedAt = seedDate,
                CreatedBy = (string?)"system",
                UpdatedAt = (DateTimeOffset?)null,
                IsDeleted = false
            },
            new
            {
                Id = "usr_underwriter_01",
                OrganizationId = "org_default",
                Email = "underwriter@bank.com",
                PasswordHash = "v1.100000.gGfggZQM6QqHijj6vf9glA==.I2hcIcH+zo25chFBSTMfEdGafdUsxevNfHNFtqBiXqY=",
                FullName = "Underwriter",
                Role = "Underwriter",
                IsActive = true,
                CreatedAt = seedDate,
                CreatedBy = (string?)"system",
                UpdatedAt = (DateTimeOffset?)null,
                IsDeleted = false
            },
            new
            {
                Id = "usr_ops_01",
                OrganizationId = "org_default",
                Email = "ops@bank.com",
                PasswordHash = "v1.100000.FefF3XKHgEB2WDp1gxMc1A==.naw8m2C279fjF9U5wsmhTTAtGMbNCclcQyaNiqQHGYE=",
                FullName = "Back Office Ops",
                Role = "BackOffice",
                IsActive = true,
                CreatedAt = seedDate,
                CreatedBy = (string?)"system",
                UpdatedAt = (DateTimeOffset?)null,
                IsDeleted = false
            },
            new
            {
                Id = "usr_customer_01",
                OrganizationId = "org_default",
                Email = "customer@bank.com",
                PasswordHash = "v1.100000.Q886Rs7thdYGS3AOKDG47A==.bZ5P+3NWhzYkcdAPAoT73cjnJRBpH8f7RyNn3GoffWM=",
                FullName = "Sample Customer",
                Role = "Customer",
                IsActive = true,
                CreatedAt = seedDate,
                CreatedBy = (string?)"system",
                UpdatedAt = (DateTimeOffset?)null,
                IsDeleted = false
            });
    }
}
