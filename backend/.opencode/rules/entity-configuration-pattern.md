# Entity Configuration Pattern

## Overview

Entities are sealed records with nested configuration classes. All entities inherit from `BaseEntity<TKey>` which provides a common key and timestamp tracking infrastructure.

## BaseEntity

```csharp
public abstract record BaseEntity<TKey> : ITimeTrackable
    where TKey : struct
{
    public TKey Id { get; init; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity<TKey>
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
```

Key design choices:
- **Abstract record** — entities are immutable data carriers
- **Generic key type** — supports `Guid`, `int`, etc. (constrained to `struct`)
- **ITimeTrackable** — interface for automatic timestamp tracking
- **Nested BaseConfiguration** — abstract class that child configurations extend

## ITimeTrackable

```csharp
public interface ITimeTrackable
{
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset UpdatedAt { get; set; }
}
```

## Concrete Entity

```csharp
public sealed record Entity : BaseEntity<Guid>
{
    public required string Name { get; set; }
    public required bool IsActive { get; set; }

    internal class Configuration : BaseConfiguration<Entity>
    {
        public override void Configure(EntityTypeBuilder<Entity> builder)
        {
            base.Configure(builder);
            builder.ToTable("Entities", DatabaseSchemas.Default);
            builder.Property(e => e.Id).HasValueGenerator<GuidV7ValueGenerator>();
        }
    }
}
```

## GuidV7 Value Generator

```csharp
internal sealed class GuidV7ValueGenerator : ValueGenerator<Guid>
{
    public override Guid Next(EntityEntry entry) => Guid.CreateVersion7();
    public override bool GeneratesTemporaryValues => false;
}
```

Uses .NET 9's native `Guid.CreateVersion7()` for time-ordered GUIDs — better for database indexing than random GUIDs.

## Automatic Timestamp Tracking

The DbContext hooks into `ChangeTracker` events to auto-populate timestamps:

```csharp
public sealed class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        TimeProvider timeProvider)
        : base(options)
    {
        this.ChangeTracker.Tracked += (_, args) => HandleStateChange(args, timeProvider);
        this.ChangeTracker.StateChanged += (_, args) => HandleStateChange(args, timeProvider);
    }

    private static void HandleStateChange(
        EntityEntryEventArgs args,
        TimeProvider timeProvider)
    {
        if (args.Entry.Entity is not ITimeTrackable timeTrackable)
            return;

        switch (args.Entry.State)
        {
            case EntityState.Added:
                timeTrackable.CreatedAt = timeProvider.GetUtcNow();
                timeTrackable.UpdatedAt = timeProvider.GetUtcNow();
                break;
            case EntityState.Modified:
                timeTrackable.UpdatedAt = timeProvider.GetUtcNow();
                break;
        }
    }
}
```

`TimeProvider` is injected for testability — can be mocked in unit tests.

## Auto-Discovery of Configurations

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
```

All nested `Configuration` classes are found automatically — no manual registration needed.

## Configuration Conventions

| Aspect | Convention |
|---|---|
| Entity type | `sealed record` extending `BaseEntity<TKey>` |
| Configuration | Nested `internal class Configuration : BaseConfiguration<TEntity>` |
| Table mapping | `builder.ToTable("TableName", DatabaseSchemas.SchemaName)` |
| Key generation | `HasValueGenerator<GuidV7ValueGenerator>()` |
| Property naming | Default EF Core conventions (PascalCase → snake_case) |
| File naming | `{EntityName}Entity.cs` |

## Schema Constants

```csharp
public static class DatabaseSchemas
{
    public const string Default = "Default";
}
```

Usage: `builder.ToTable("Entities", DatabaseSchemas.Default);`
