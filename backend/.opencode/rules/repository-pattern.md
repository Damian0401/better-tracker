# Repository Pattern

## Overview

Standard repository pattern using entity-specific repositories for commands. This provides a clean, explicit abstraction for writes while maintaining strong typing and testability. Queries continue to use `DbContext` directly via LINQ.

## Repository Interface

Create a domain-specific interface for each aggregate root:

```csharp
public interface IEntityRepository
{
    Task<Entity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Entity?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    void Add(Entity entity);
    void Update(Entity entity);
    void Remove(Entity entity);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

## Implementation

```csharp
public sealed class EntityRepository : IEntityRepository
{
    private readonly AppDbContext _dbContext;

    public EntityRepository(AppDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<Entity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await this._dbContext.Entities.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Entity?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await this._dbContext.Entities.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public void Add(Entity entity)
    {
        this._dbContext.Entities.Add(entity);
    }

    public void Update(Entity entity)
    {
        this._dbContext.Entities.Update(entity);
    }

    public void Remove(Entity entity)
    {
        this._dbContext.Entities.Remove(entity);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await this._dbContext.SaveChangesAsync(cancellationToken);
    }
}
```

## Registration

Register repositories for each aggregate root:

```csharp
public static class Setup
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IEntityRepository, EntityRepository>();
        // Register additional repositories as needed
        return services;
    }
}
```

Usage in `Program.cs`:

```csharp
builder.Services.AddRepositories();
```

## Usage

### In Commands

Inject the specific repository interface:

```csharp
public static class CreateEntity
{
    public static async Task HandleAsync(
        CreateEntityRequest request,
        IEntityRepository entityRepository,
        CancellationToken cancellationToken)
    {
        var entity = new Entity
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            IsActive = request.IsActive,
        };

        entityRepository.Add(entity);
        await entityRepository.SaveChangesAsync(cancellationToken);
    }
}
```

### In Queries

Use `DbContext` directly for optimal LINQ composition:

```csharp
public static class ListEntities
{
    public static async ValueTask<ListEntitiesResponse> HandleAsync(
        int? count,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var take = count ?? 5;

        var items = await dbContext.Entities
            .Where(x => x.IsActive)
            .Take(take)
            .Select(x => new ListEntitiesItemDto
            {
                Id = x.Id,
                Name = x.Name,
                IsActive = x.IsActive,
            })
            .ToListAsync(cancellationToken);

        return new ListEntitiesResponse { Items = items };
    }
}
```

## Why This Pattern?

| Aspect | Benefit |
|---|---|
| **Entity-specific repositories** | Strong typing, explicit intent, easier to maintain |
| **Simple interfaces** | Only methods needed for each aggregate root, no generic bloat |
| **Commands use repository** | Easy to mock for unit tests, hides persistence details |
| **Queries use DbContext** | Full LINQ power, composable, no additional abstraction needed |
| **Clear separation** | Write operations via repository, read operations via DbContext |
| **Testable** | Mock entity-specific repositories in unit tests, no database needed |
| **Standard practice** | Explicit repositories are the industry standard for DDD |
