using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Application.Features.Batch.Commands.GenerateDailyAccruals;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace LoanManagement.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public sealed class DailyInterestAccrualJob : IJob
{
    public const string JobKeyName = "daily-interest-accrual";

    private readonly ISender _sender;
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;
    private readonly ILogger<DailyInterestAccrualJob> _logger;

    public DailyInterestAccrualJob(ISender sender, IApplicationDbContext db, IDateTimeProvider clock, ILogger<DailyInterestAccrualJob> logger)
    {
        _sender = sender;
        _db = db;
        _clock = clock;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var accrualDate = _clock.UtcToday;
        var orgIds = await _db.Organizations.AsNoTracking().Where(x => x.IsActive).Select(x => x.Id).ToListAsync(context.CancellationToken);
        foreach (var orgId in orgIds)
        {
            var result = await _sender.Send(new GenerateDailyAccrualsCommand(orgId, accrualDate), context.CancellationToken);
            _logger.LogInformation("Daily accrual completed for org {OrgId}: loans={Loans}, total={Total}, batch={Batch}", orgId, result.ProcessedLoansCount, result.TotalAccruedInterest, result.BatchRunId);
        }
    }
}
