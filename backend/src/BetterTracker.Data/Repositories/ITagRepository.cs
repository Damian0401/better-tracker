using BetterTracker.Data.Entities;

namespace BetterTracker.Data.Repositories;

public interface ITagRepository
{
    Task<TagEntity?> GetByUserIdAndNameAsync(Guid userId, string name, CancellationToken cancellationToken = default);
    Task RemoveOrphanedByUserIdAsync(
        Guid userId,
        IReadOnlyCollection<Guid> candidateTagIds,
        Guid excludedJobApplicationId,
        CancellationToken cancellationToken = default);
    void Add(TagEntity tag);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
