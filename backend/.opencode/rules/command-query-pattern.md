# Command/Query Pattern

## Overview

Commands and queries are public static classes with a single `HandleAsync` method. They contain business logic and are called from endpoint handlers. Commands use repositories for clean, testable data access. Queries use `DbContext` directly for optimal LINQ composition.

## Command Pattern

Commands mutate state using a repository. Return type is flexible — use `Task` for fire-and-forget operations, or `Task<T>` / `ValueTask<T>` when a result is needed.

```csharp
public static class CreateEntity
{
    public static async Task HandleAsync(
        CreateEntityRequest request,
        IRepository repository,
        CancellationToken cancellationToken)
    {
        var entity = new Entity
        {
            Name = request.Name,
            IsActive = request.IsActive,
        };

        repository.Add(entity);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
```

### Command with Return Value

```csharp
public static class CreateEntityWithResult
{
    public static async Task<Guid> HandleAsync(
        CreateEntityRequest request,
        IRepository repository,
        CancellationToken cancellationToken)
    {
        var entity = new Entity
        {
            Name = request.Name,
            IsActive = request.IsActive,
        };

        repository.Add(entity);
        await repository.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
```

## Query Pattern

Queries read data and return `ValueTask<TResponse>`. They use `DbContext` directly for composable LINQ queries.

```csharp
public static class ListEntities
{
    private const int DefaultCount = 5;

    public static async ValueTask<ListEntitiesResponse> HandleAsync(
        int? count,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var take = count ?? DefaultCount;

        var items = await dbContext.Set<Entity>()
            .Where(x => x.IsActive)
            .Take(take)
            .Select(x => new ListEntitiesResponse.Dto
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

## Conventions

| Aspect | Convention |
|---|---|
| Class type | `public static class` |
| Method name | `HandleAsync` |
| Return type | `Task` or `Task<T>` (commands), `ValueTask<TResponse>` (queries) |
| Parameters | Request DTO, `IRepository` or `DbContext`, `CancellationToken` |
| Namespace | `{Root}.Core.{Domain}.{Commands\|Queries}` |
| Commands use | `IRepository` for mutations |
| Queries use | `DbContext` directly for LINQ queries |

## Why Commands Use IRepository and Queries Use DbContext

- **Commands** use `IRepository` for a clean, mockable abstraction that hides persistence details. Easy to unit test without a database.
- **Queries** use `DbContext` directly to access the full power of LINQ and EF Core's query translation. No abstraction needed for read operations.

## Calling from Endpoints

### Command (no return)

```csharp
private static async ValueTask<NoContent> HandleAsync(
    [AsParameters] Parameters parameters,
    [AsParameters] Services services)
{
    await CreateEntity.HandleAsync(
        parameters.Request,
        services.Repository,
        services.CancellationToken);

    return TypedResults.NoContent();
}
```

### Command (with return)

```csharp
private static async ValueTask<Ok<CreatedEntityResponse>> HandleAsync(
    [AsParameters] Parameters parameters,
    [AsParameters] Services services)
{
    var id = await CreateEntityWithResult.HandleAsync(
        parameters.Request,
        services.Repository,
        services.CancellationToken);

    return TypedResults.Ok(new CreatedEntityResponse { Id = id });
}
```

### Query

```csharp
private static async ValueTask<Ok<ListEntitiesResponse>> HandleAsync(
    [AsParameters] Parameters parameters,
    [AsParameters] Services services)
{
    var response = await ListEntities.HandleAsync(
        parameters.Count,
        services.DbContext,
        services.CancellationToken);

    return TypedResults.Ok(response);
}
```

## Namespace Structure

```
{Root}.Core/
  {Domain}/
    Commands/
      CreateEntity.cs
      UpdateEntity.cs
      DeleteEntity.cs
    Queries/
      ListEntities.cs
      GetEntityById.cs
```

See [testing-patterns.md](./testing-patterns.md) for xUnit testing guidelines.
