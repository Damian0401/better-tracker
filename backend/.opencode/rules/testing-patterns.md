# Testing Patterns

## Overview

Tests live in `tests/` directory using xUnit, FluentAssertions, and NSubstitute. Only commands are unit tested. Queries are not tested with in-memory databases as this is an anti-pattern.

## Test Project Setup

```bash
dotnet new xunit -n {Root}.Tests -o tests/{Root}.Tests
dotnet add tests/{Root}.Tests reference src/{Root}.Core src/{Root}.Contracts src/{Root}.Data
dotnet add tests/{Root}.Tests package FluentAssertions
dotnet add tests/{Root}.Tests package NSubstitute
```

## Testing Commands

Commands use entity-specific repositories which are mocked with NSubstitute. Verify `Add`, `Update`, `Remove`, and `SaveChangesAsync` calls.

```csharp
public class CreateEntityTests
{
    private readonly IEntityRepository _entityRepository;

    public CreateEntityTests()
    {
        this._entityRepository = Substitute.For<IEntityRepository>();
    }

    [Fact]
    public async Task HandleAsync_ShouldAddEntity_WhenCalled()
    {
        // Arrange
        var request = new CreateEntityRequest
        {
            Name = "Test",
            IsActive = true,
        };

        // Act
        await CreateEntity.HandleAsync(request, this._entityRepository, CancellationToken.None);

        // Assert
        this._entityRepository.Received(1).Add(Arg.Any<Entity>());
        await this._entityRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldSetCorrectProperties_WhenCalled()
    {
        // Arrange
        var request = new CreateEntityRequest
        {
            Name = "Test",
            IsActive = true,
        };

        Entity? captured = null;
        this._entityRepository.When(x => x.Add(Arg.Any<Entity>()))
            .Do(x => captured = x.Arg<Entity>());

        // Act
        await CreateEntity.HandleAsync(request, this._entityRepository, CancellationToken.None);

        // Assert
        captured.Should().NotBeNull();
        captured!.Name.Should().Be("Test");
        captured.IsActive.Should().BeTrue();
    }
}
```

## Testing Update Commands

```csharp
public class UpdateEntityTests
{
    private readonly IEntityRepository _entityRepository;

    public UpdateEntityTests()
    {
        this._entityRepository = Substitute.For<IEntityRepository>();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateEntity_WhenEntityExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingEntity = new Entity { Id = id, Name = "Old Name", IsActive = false };
        this._entityRepository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Entity?>(existingEntity));

        var request = new UpdateEntityRequest
        {
            Id = id,
            Name = "New Name",
            IsActive = true,
        };

        Entity? captured = null;
        this._entityRepository.When(x => x.Update(Arg.Any<Entity>()))
            .Do(x => captured = x.Arg<Entity>());

        // Act
        await UpdateEntity.HandleAsync(request, this._entityRepository, CancellationToken.None);

        // Assert
        captured.Should().NotBeNull();
        captured!.Name.Should().Be("New Name");
        captured.IsActive.Should().BeTrue();
        await this._entityRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
```

## Testing Delete Commands

```csharp
public class DeleteEntityTests
{
    private readonly IEntityRepository _entityRepository;

    public DeleteEntityTests()
    {
        this._entityRepository = Substitute.For<IEntityRepository>();
    }

    [Fact]
    public async Task HandleAsync_ShouldRemoveEntity_WhenEntityExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new Entity { Id = id, Name = "Test", IsActive = true };
        this._entityRepository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Entity?>(entity));

        var request = new DeleteEntityRequest { Id = id };

        // Act
        await DeleteEntity.HandleAsync(request, this._entityRepository, CancellationToken.None);

        // Assert
        this._entityRepository.Received(1).Remove(Arg.Is(entity));
        await this._entityRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
```

## Conventions

| Aspect | Convention |
|---|---|
| Framework | xUnit |
| Assertions | FluentAssertions |
| Mocking | NSubstitute |
| Test class name | `{CommandName}Tests` |
| Test method name | `HandleAsync_Should{ExpectedBehavior}_When{Condition}` |
| Commands | Mock entity-specific repositories, no database |
| Queries | Integration tested in a staging/production environment, not unit tested |
| Arrange | Set up mocks and request objects |
| Act | Call `HandleAsync` |
| Assert | Verify mock interactions and command behavior |
| CancellationToken | Use `CancellationToken.None` unless testing cancellation |

## Test File Structure

```
tests/{Root}.Tests/
  Commands/
    CreateEntityTests.cs
    UpdateEntityTests.cs
    DeleteEntityTests.cs
```

## Why This Approach?

| Aspect | Rationale |
|---|---|
| **Commands with mocks** | Commands have business logic that must be tested; mocking repositories allows fast, isolated tests |
| **No query tests** | Queries are thin LINQ compositions; testing them with in-memory databases is an anti-pattern that doesn't catch real database issues |
| **Integration testing** | Queries are best validated through integration tests or staging environment testing against a real database |
| **Fast feedback** | Unit tests run instantly with mocked repositories; provides immediate feedback during development |
| **Clear focus** | Only test what changes: business logic in commands, not data access layer |
