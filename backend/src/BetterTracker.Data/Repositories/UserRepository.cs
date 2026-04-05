using BetterTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BetterTracker.Data.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await this.dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<UserEntity?> GetByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        return await this.dbContext.Users.FirstOrDefaultAsync(x => x.Login == login, cancellationToken);
    }

    public void Add(UserEntity user)
    {
        this.dbContext.Users.Add(user);
    }

    public void Update(UserEntity user)
    {
        this.dbContext.Users.Update(user);
    }

    public void Remove(UserEntity user)
    {
        this.dbContext.Users.Remove(user);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await this.dbContext.SaveChangesAsync(cancellationToken);
    }
}
