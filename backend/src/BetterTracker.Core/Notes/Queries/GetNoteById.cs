using BetterTracker.Contracts;
using BetterTracker.Data;
using BetterTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BetterTracker.Core.Notes.Queries;

public static class GetNoteById
{
    public static async ValueTask<GetNoteByIdResponse?> HandleAsync(
        Guid id,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var note = await dbContext.Notes
            .Where(x => x.Id == id)
            .Select(x => new GetNoteByIdResponse.Dto
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (note is null)
        {
            return null;
        }

        return new GetNoteByIdResponse { Note = note };
    }
}
