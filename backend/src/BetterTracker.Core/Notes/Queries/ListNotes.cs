using BetterTracker.Contracts;
using BetterTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace BetterTracker.Core.Notes.Queries;

public static class ListNotes
{
    private const int DefaultCount = 10;

    public static async ValueTask<ListNotesResponse> HandleAsync(
        int? count,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var take = count ?? DefaultCount;

        var items = await dbContext.Notes
            .OrderByDescending(x => x.CreatedAt)
            .Take(take)
            .Select(x => new ListNotesResponse.Dto
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            })
            .ToListAsync(cancellationToken);

        return new ListNotesResponse { Items = items };
    }
}
