# Result Pattern

## Overview

This codebase uses the **Result pattern** instead of throwing exceptions for normal control flow. Exceptions are reserved for truly exceptional situations (infrastructure failures, unrecoverable errors, etc.). Business logic validation and expected error cases return `Result` or `Result<T>` types.

## Core Result Types

### Result (Non-Generic)

Used for operations that don't return data, only success/failure status.

```csharp
public sealed record Result
{
    public bool IsSuccess { get; init; }
    public string[] ErrorMessages { get; init; } = [];
    
    public static Result Success() => new() { IsSuccess = true };
    
    public static Result Failure(params string[] errorMessages) => new()
    {
        IsSuccess = false,
        ErrorMessages = errorMessages
    };
    
    public static Result Failure(IEnumerable<string> errorMessages) => new()
    {
        IsSuccess = false,
        ErrorMessages = errorMessages.ToArray()
    };
}
```

### Result<T> (Generic)

Used for operations that return data on success.

```csharp
public sealed record Result<T>
{
    public bool IsSuccess { get; init; }
    public string[] ErrorMessages { get; init; } = [];
    public T? Data { get; init; }
    
    public static Result<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data
    };
    
    public static Result<T> Failure(params string[] errorMessages) => new()
    {
        IsSuccess = false,
        ErrorMessages = errorMessages
    };
    
    public static Result<T> Failure(IEnumerable<string> errorMessages) => new()
    {
        IsSuccess = false,
        ErrorMessages = errorMessages.ToArray()
    };
}
```

## Location

Result types should be defined in `src/BetterTracker.Common/Results/`:
- `Result.cs` - Non-generic result
- `Result{T}.cs` - Generic result

## Usage Patterns

### Commands

Commands should return `Task<Result>` or `Task<Result<T>>`:

```csharp
public static class CreateEntity
{
    public static async Task<Result<Guid>> HandleAsync(
        CreateEntityRequest request,
        IEntityRepository repository,
        CancellationToken cancellationToken)
    {
        // Validation or business rule check
        var existingEntity = await repository.GetByNameAsync(request.Name, cancellationToken);
        if (existingEntity is not null)
        {
            return Result<Guid>.Failure("An entity with this name already exists.");
        }
        
        var entity = new EntityEntity
        {
            Id = Guid.CreateVersion7(),
            Name = request.Name
        };
        
        repository.Add(entity);
        await repository.SaveChangesAsync(cancellationToken);
        
        return Result<Guid>.Success(entity.Id);
    }
}
```

### Queries

Queries should return `ValueTask<Result<TResponse>>`:

```csharp
public static class GetEntityById
{
    public static async ValueTask<Result<EntityResponse>> HandleAsync(
        Guid id,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var entity = await dbContext.Entities
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        
        if (entity is null)
        {
            return Result<EntityResponse>.Failure($"Entity with ID {id} not found.");
        }
        
        var response = new EntityResponse
        {
            Id = entity.Id,
            Name = entity.Name
        };
        
        return Result<EntityResponse>.Success(response);
    }
}
```

### Endpoints

Endpoints should check the result and return appropriate HTTP responses:

```csharp
public sealed class CreateEntityEndpoint : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/entities", this.HandleAsync)
            .WithName(nameof(CreateEntityEndpoint))
            .WithOpenApi()
            .AddEndpointFilter<ValidationFilter<CreateEntityRequest>>();
    }
    
    internal async ValueTask<IResult> HandleAsync(
        Parameters parameters,
        Services services)
    {
        var result = await CreateEntity.HandleAsync(
            parameters.Request,
            services.Repository,
            services.CancellationToken);
        
        if (!result.IsSuccess)
        {
            return Results.BadRequest(new { errors = result.ErrorMessages });
        }
        
        return Results.Created($"/entities/{result.Data}", new { id = result.Data });
    }
    
    internal readonly record struct Parameters(
        [AsParameters] CreateEntityRequest Request);
    
    internal readonly record struct Services(
        IEntityRepository Repository,
        HttpContext HttpContext)
    {
        public CancellationToken CancellationToken => this.HttpContext.RequestAborted;
    }
}
```

### Multiple Errors

You can accumulate multiple errors:

```csharp
var errors = new List<string>();

if (string.IsNullOrWhiteSpace(request.Name))
{
    errors.Add("Name is required.");
}

if (request.Name.Length > 100)
{
    errors.Add("Name must not exceed 100 characters.");
}

if (errors.Any())
{
    return Result.Failure(errors);
}
```

## HTTP Status Code Mapping

Map result failures to appropriate HTTP status codes:

| Scenario | Status Code | Method |
|----------|-------------|--------|
| Validation errors | 400 Bad Request | `Results.BadRequest()` |
| Not found | 404 Not Found | `Results.NotFound()` |
| Conflict (duplicate) | 409 Conflict | `Results.Conflict()` |
| Unauthorized | 401 Unauthorized | `Results.Unauthorized()` |
| Forbidden | 403 Forbidden | `Results.Forbid()` |
| Generic failure | 400 Bad Request | `Results.BadRequest()` |

Example with different status codes:

```csharp
internal async ValueTask<IResult> HandleAsync(
    Parameters parameters,
    Services services)
{
    var result = await GetEntityById.HandleAsync(
        parameters.Id,
        services.DbContext,
        services.CancellationToken);
    
    if (!result.IsSuccess)
    {
        // Could check error message content to determine appropriate status code
        if (result.ErrorMessages.Any(e => e.Contains("not found")))
        {
            return Results.NotFound(new { errors = result.ErrorMessages });
        }
        
        return Results.BadRequest(new { errors = result.ErrorMessages });
    }
    
    return Results.Ok(result.Data);
}
```

## When to Throw Exceptions

**DO** throw exceptions for:
- Infrastructure failures (database connection lost, file system errors)
- Programming errors (null reference when not expected, invalid cast)
- Unrecoverable errors (out of memory, stack overflow)
- Third-party library exceptions that should bubble up

**DO NOT** throw exceptions for:
- Validation errors (invalid input)
- Business rule violations (duplicate name, insufficient balance)
- Expected "not found" scenarios
- Any normal control flow

## Testing

### Testing Commands that Return Result

```csharp
[Fact]
public async Task HandleAsync_ShouldReturnFailure_WhenEntityAlreadyExists()
{
    // Arrange
    var request = new CreateEntityRequest { Name = "Test" };
    var repository = Substitute.For<IEntityRepository>();
    repository.GetByNameAsync(request.Name, Arg.Any<CancellationToken>())
        .Returns(new EntityEntity { Id = Guid.NewGuid(), Name = "Test" });
    
    // Act
    var result = await CreateEntity.HandleAsync(
        request,
        repository,
        CancellationToken.None);
    
    // Assert
    result.IsSuccess.Should().BeFalse();
    result.ErrorMessages.Should().Contain("An entity with this name already exists.");
}

[Fact]
public async Task HandleAsync_ShouldReturnSuccess_WhenEntityIsCreated()
{
    // Arrange
    var request = new CreateEntityRequest { Name = "Test" };
    var repository = Substitute.For<IEntityRepository>();
    repository.GetByNameAsync(request.Name, Arg.Any<CancellationToken>())
        .Returns((EntityEntity?)null);
    
    // Act
    var result = await CreateEntity.HandleAsync(
        request,
        repository,
        CancellationToken.None);
    
    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Data.Should().NotBeEmpty();
}
```

## Best Practices

1. **Always check IsSuccess** before accessing Data in `Result<T>`
2. **Provide meaningful error messages** that can be displayed to users
3. **Use array for multiple errors** to return all validation failures at once
4. **Keep error messages user-friendly** (avoid technical details/stack traces)
5. **Be consistent** - all commands/queries in a feature should use the same pattern
6. **Don't mix patterns** - don't return Result sometimes and throw exceptions other times for the same type of error
7. **Map to appropriate HTTP status codes** in endpoints based on error type

## Anti-Patterns

**DON'T** access Data without checking IsSuccess:
```csharp
// BAD
var result = await GetEntity.HandleAsync(id, dbContext, cancellationToken);
var entity = result.Data; // May be null if IsSuccess is false!
```

**DON'T** throw exceptions for validation:
```csharp
// BAD
if (existingEntity is not null)
{
    throw new InvalidOperationException("Entity already exists");
}

// GOOD
if (existingEntity is not null)
{
    return Result.Failure("Entity already exists");
}
```

**DON'T** return success with null data:
```csharp
// BAD
return Result<Entity>.Success(null); // Data should never be null on success

// GOOD
return Result<Entity>.Failure("Entity not found");
```

**DON'T** use empty error messages:
```csharp
// BAD
return Result.Failure();

// GOOD
return Result.Failure("Operation failed due to validation error");
```
