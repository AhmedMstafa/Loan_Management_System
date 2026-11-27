using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LoanManagement.Infrastructure.Persistence.Interceptors;

public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTime;

    public AuditableEntityInterceptor(ICurrentUserService currentUser, IDateTimeProvider dateTime)
    {
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context is null) return;

        var utcNow = _dateTime.UtcNow;
        var userId = _currentUser.UserId;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is not IHasDomainEvents)
                continue;

            var createdAt = entry.Property(nameof(BaseEntity<string>.CreatedAt));
            var createdBy = entry.Property(nameof(BaseEntity<string>.CreatedBy));
            var updatedAt = entry.Property(nameof(BaseEntity<string>.UpdatedAt));

            if (entry.State == EntityState.Added)
            {
                if (createdAt.CurrentValue is DateTimeOffset dto && dto == default)
                    createdAt.CurrentValue = utcNow;
                if (createdBy.CurrentValue is null && !string.IsNullOrWhiteSpace(userId))
                    createdBy.CurrentValue = userId;
            }

            if (entry.State == EntityState.Modified)
            {
                updatedAt.CurrentValue = utcNow;
            }
        }
    }
}
