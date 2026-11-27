using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Application.Features.Batch.Commands.RunBatchBilling;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace LoanManagement.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public sealed class MonthlyBillingJob : IJob
{
    public const string JobKeyName = "monthly-billing";

    private readonly ISender _sender;
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;
    private readonly ILogger<MonthlyBillingJob> _logger;

    public MonthlyBillingJob(ISender sender, IApplicationDbContext db, IDateTimeProvider clock, ILogger<MonthlyBillingJob> logger)
    {
        _sender = sender;
        _db = db;
        _clock = clock;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var cycleDate = _clock.UtcToday;
        var orgIds = await _db.Organizations.AsNoTracking().Where(x => x.IsActive).Select(x => x.Id).ToListAsync(context.CancellationToken);
        foreach (var orgId in orgIds)
        {
            var result = await _sender.Send(new RunBatchBillingCommand(orgId, cycleDate), context.CancellationToken);
            _logger.LogInformation("Monthly billing completed for org {OrgId}: statements={Statements}, status={Status}", orgId, result.StatementsGenerated, result.Status);
        }
    }
}
