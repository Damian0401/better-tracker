using BetterTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BetterTracker.Data.Repositories;

public sealed class TagRepository : ITagRepository
{
    private readonly AppDbContext dbContext;

    public TagRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<TagEntity?> GetByUserIdAndNameAsync(Guid userId, string name, CancellationToken cancellationToken = default)
    {
        return await this.dbContext.Tags
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Name == name, cancellationToken);
    }

    public async Task RemoveOrphanedByUserIdAsync(
        Guid userId,
        IReadOnlyCollection<Guid> candidateTagIds,
        Guid excludedJobApplicationId,
        CancellationToken cancellationToken = default)
    {
        if (candidateTagIds.Count == 0)
        {
            return;
        }

        var orphanedTags = await this.dbContext.Tags
            .Where(x => x.UserId == userId && candidateTagIds.Contains(x.Id))
            .Where(x => !this.dbContext.JobApplicationTags
                .Any(jat => jat.TagId == x.Id && jat.JobApplicationId != excludedJobApplicationId))
            .ToListAsync(cancellationToken);

        if (orphanedTags.Count > 0)
        {
            this.dbContext.Tags.RemoveRange(orphanedTags);
        }
    }

    public void Add(TagEntity tag)
    {
        this.dbContext.Tags.Add(tag);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await this.dbContext.SaveChangesAsync(cancellationToken);
    }
}
