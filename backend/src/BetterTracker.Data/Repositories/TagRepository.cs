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

    public void Add(TagEntity tag)
    {
        this.dbContext.Tags.Add(tag);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await this.dbContext.SaveChangesAsync(cancellationToken);
    }
}
