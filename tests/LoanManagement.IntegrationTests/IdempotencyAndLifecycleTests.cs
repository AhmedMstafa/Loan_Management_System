using FluentAssertions;
using FluentValidation;
using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Application.Features.Loans.Commands.ApproveLoan;
using LoanManagement.Application.Features.Loans.Commands.CreateLoanApplication;
using LoanManagement.Application.Features.Loans.Commands.DisburseLoan;
using LoanManagement.Application.Features.Repayments.Commands.MakeRepayment;
using LoanManagement.Domain.Entities;
using LoanManagement.Domain.Enums;
using LoanManagement.Infrastructure.Persistence;
using LoanManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LoanManagement.IntegrationTests;

public class IdempotencyAndLifecycleTests
{
    [Fact]
    public async Task IdempotencyService_ShouldReturnCachedResponse_ForDuplicateKey()
    {
        await using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var svc = scope.ServiceProvider.GetRequiredService<IIdempotencyService>();
        const string orgId = "org1";
        const string key = "idem-1";
        const string hash = "hash-1";

        await svc.StoreAsync(orgId, key, hash, 200, "{\"ok\":true}", TimeSpan.FromMinutes(10));
        var cached = await svc.GetAsync(orgId, key, hash);

        cached.Should().NotBeNull();
        cached!.StatusCode.Should().Be(200);
        cached.ResponsePayload.Should().Be("{\"ok\":true}");
    }

    [Fact]
    public async Task LoanLifecycle_ShouldCreateApproveDisburseAndRepay()
    {
        await using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.EnsureCreatedAsync();
        db.Organizations.Add(new Organization("org1", "Test Org"));
        db.Borrowers.Add(new Borrower("bor_1", "org1", "Jane", "Doe", "jane@example.com", "555-0100", "hash", 5000m));
        db.LoanProducts.Add(new LoanProduct("prod_1", "org1", "Personal", 12m, 1000m, 50000m, 6, 60, 1m, 25m));
        await db.SaveChangesAsync();

        var appDb = (IApplicationDbContext)db;
        var clock = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        var createHandler = new CreateLoanCommandHandler(appDb, clock);
        var created = await createHandler.Handle(new CreateLoanCommand("org1", "bor_1", "prod_1", 10000m, "USD", 12, 12m, "Test", null), CancellationToken.None);
        created.Status.Should().Be(nameof(LoanStatus.Draft));

        var approveHandler = new ApproveLoanCommandHandler(appDb, clock);
        var approved = await approveHandler.Handle(new ApproveLoanCommand("org1", created.Id, 10000m, 12m, 12, "ok", new DateOnly(2026, 1, 10), "uw1"), CancellationToken.None);
        approved.Status.Should().Be(nameof(LoanStatus.Approved));

        var disburseHandler = new DisburseLoanCommandHandler(appDb, clock);
        var disbursed = await disburseHandler.Handle(new DisburseLoanCommand("org1", created.Id, 10000m, "BankTransfer", "001", "123", "Borrower", new DateOnly(2026, 2, 10)), CancellationToken.None);
        disbursed.Status.Should().Be(nameof(LoanStatus.Disbursed));

        var repayHandler = new MakeRepaymentCommandHandler(appDb, clock);
        var repaid = await repayHandler.Handle(new MakeRepaymentCommand("org1", created.Id, 1200m, PaymentMethod.BankTransfer, "TXN-001", DateTimeOffset.UtcNow), CancellationToken.None);
        repaid.AmountPaid.Should().Be(1200m);
        repaid.NewOutstandingBalance.Should().BeLessThan(10000m);
    }

    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<ApplicationDbContext>(o =>
            o.UseInMemoryDatabase($"lms-tests-{Guid.NewGuid()}")
             .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning)));
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<ICurrentUserService, TestCurrentUserService>();
        services.AddScoped<IIdempotencyService, IdempotencyService>();
        services.AddScoped<ICreditBureauService, CreditBureauService>();
        services.AddScoped<IDocumentStorageService, DocumentStorageService>();

        return services.BuildServiceProvider();
    }

    private sealed class TestCurrentUserService : ICurrentUserService
    {
        public string? UserId => "test-user";
        public string? OrganizationId => "org1";
        public string? Role => "Admin";
        public bool IsAuthenticated => true;
    }
}
