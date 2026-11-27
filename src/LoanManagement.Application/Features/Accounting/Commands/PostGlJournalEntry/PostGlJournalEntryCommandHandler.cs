using LoanManagement.Application.Common.Interfaces;
using LoanManagement.Domain.Entities;
using MediatR;

namespace LoanManagement.Application.Features.Accounting.Commands.PostGlJournalEntry;

public sealed class PostGlJournalEntryCommandHandler : IRequestHandler<PostGlJournalEntryCommand, PostGlJournalEntryResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;
    public PostGlJournalEntryCommandHandler(IApplicationDbContext db, IDateTimeProvider clock) { _db = db; _clock = clock; }

    public async Task<PostGlJournalEntryResponse> Handle(PostGlJournalEntryCommand request, CancellationToken cancellationToken)
    {
        var now = _clock.UtcNow;
        var entry = new GlJournalEntry($"GL_{Guid.NewGuid():N}"[..16], request.OrganizationId, request.AccountingDate, request.Memo);
        foreach (var line in request.Lines)
            entry.AddLine(line.AccountNumber, line.Debit, line.Credit);
        entry.ValidateDoubleEntry();
        entry.MarkCreated(now);

        _db.GlJournalEntries.Add(entry);
        await _db.SaveChangesAsync(cancellationToken);
        return new PostGlJournalEntryResponse(entry.Id, entry.Status, entry.TotalDebit, entry.TotalCredit, now);
    }
}
