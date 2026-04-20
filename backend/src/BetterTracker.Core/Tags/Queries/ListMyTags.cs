using BetterTracker.Contracts;
using BetterTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace BetterTracker.Core.Tags.Queries;

public static class ListMyTags
{
    public static async ValueTask<ListMyTagsResponse> HandleAsync(
        Guid userId,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var items = await dbContext.Tags
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.Name)
            .Select(x => new ListMyTagsItemDto
            {
                Id = x.Id,
                Name = x.Name,
            })
            .ToListAsync(cancellationToken);

        return new ListMyTagsResponse
        {
            Items = items,
        };
    }
}
