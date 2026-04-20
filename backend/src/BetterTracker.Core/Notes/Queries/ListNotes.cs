using BetterTracker.Contracts;
using BetterTracker.Data;
using BetterTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BetterTracker.Core.Notes.Queries;

public static class ListNotes
{
    private const int DefaultCount = 10;

    public static async ValueTask<ListNotesResponse> HandleAsync(
        int? count,
        int? skip,
        Guid userId,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var take = count ?? DefaultCount;
        var skipCount = skip ?? 0;

        var total = await dbContext.Notes
            .Where(x => x.UserId == userId)
            .CountAsync(cancellationToken);

        var items = await dbContext.Notes
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip(skipCount)
            .Take(take)
            .Select(x => new ListNotesItemDto
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            })
            .ToListAsync(cancellationToken);

        return new ListNotesResponse { Total = total, Items = items };
    }
}
