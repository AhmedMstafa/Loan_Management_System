using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Infrastructure.Jobs;
using LoanManagement.Infrastructure.Persistence;
using LoanManagement.Infrastructure.Persistence.Interceptors;
using LoanManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace LoanManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntityInterceptor>();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=LoanManagementDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
            options.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddHttpContextAccessor();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ICreditBureauService, CreditBureauService>();
        services.AddScoped<IDocumentStorageService, DocumentStorageService>();
        services.AddScoped<IIdempotencyService, IdempotencyService>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddQuartz(q =>
        {
            var accrualJobKey = new JobKey(DailyInterestAccrualJob.JobKeyName);
            q.AddJob<DailyInterestAccrualJob>(opts => opts.WithIdentity(accrualJobKey));
            q.AddTrigger(opts => opts
                .ForJob(accrualJobKey)
                .WithIdentity($"{DailyInterestAccrualJob.JobKeyName}-trigger")
                .WithCronSchedule("0 0 0 * * ?", x => x.InTimeZone(TimeZoneInfo.Utc)));

            var billingJobKey = new JobKey(MonthlyBillingJob.JobKeyName);
            q.AddJob<MonthlyBillingJob>(opts => opts.WithIdentity(billingJobKey));
            q.AddTrigger(opts => opts
                .ForJob(billingJobKey)
                .WithIdentity($"{MonthlyBillingJob.JobKeyName}-trigger")
                .WithCronSchedule("0 0 0 1 * ?", x => x.InTimeZone(TimeZoneInfo.Utc)));
        });
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        return services;
    }
}
