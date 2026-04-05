using BetterTracker.Data.Entities;

namespace BetterTracker.Data.Repositories;

public interface IUserRepository
{
    Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByLoginAsync(string login, CancellationToken cancellationToken = default);
    void Add(UserEntity user);
    void Update(UserEntity user);
    void Remove(UserEntity user);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
