using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Common.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IApplicationDbContext _db;

    public TransactionBehavior(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ITransactionalCommand || _db is not DbContext dbContext)
            return await next(cancellationToken);

        if (dbContext.Database.CurrentTransaction is not null)
            return await next(cancellationToken);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var response = await next(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return response;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
