# Agent Guide for Better Tracker Backend

This guide provides essential information for AI coding agents working on this .NET 9.0 ASP.NET Core Web API codebase.

## Quick Reference

- **Framework**: .NET 9.0, ASP.NET Core Web API
- **Architecture**: Clean Architecture with minimal endpoints and EF Core
- **Database**: SQLite with EF Core
- **Testing**: xUnit, FluentAssertions, NSubstitute
- **Solution Root**: `/backend`

## Build, Test, and Run Commands

### Build Commands

```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build src/BetterTracker

# Clean and rebuild
dotnet clean && dotnet build
```

### Run Commands

```bash
# Run the application (from solution root)
dotnet run --project src/BetterTracker

# Run with hot reload
dotnet watch --project src/BetterTracker
```

### Test Commands

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run a single test by name
dotnet test --filter "FullyQualifiedName~CreateEntityTests"

# Run a specific test method
dotnet test --filter "FullyQualifiedName=BetterTracker.Tests.Commands.CreateEntityTests.HandleAsync_ShouldAddEntity_WhenCalled"

# Run tests matching a pattern
dotnet test --filter "DisplayName~CreateEntity"
```

### Database Commands

```bash
# Add a migration
dotnet ef migrations add <MigrationName> --project src/BetterTracker.Data.Migrations --startup-project src/BetterTracker

# Update database
dotnet ef database update --project src/BetterTracker.Data.Migrations --startup-project src/BetterTracker

# Remove last migration
dotnet ef migrations remove --project src/BetterTracker.Data.Migrations --startup-project src/BetterTracker

# List migrations
dotnet ef migrations list --project src/BetterTracker.Data.Migrations --startup-project src/BetterTracker
```

## Architecture Patterns

This codebase follows specific architectural patterns documented in `.opencode/rules/`. **Always review these rules before making changes:**

### Core Documentation

1. **[Clean Architecture Structure](.opencode/rules/clean-architecture-structure.md)**
   - Project structure and dependencies
   - Layer responsibilities (Api, Core, Contracts, Data, Common)
   - Extension method patterns for setup (`AddX`, `UseX`, `MapX`)

2. **[Endpoint Patterns](.opencode/rules/endpoint-patterns.md)**
   - `IApiEndpoint` interface implementation
   - Automatic endpoint discovery via reflection
   - Parameters, Services, and Validator nested types
   - Validation filters with FluentValidation

3. **[Command/Query Pattern](.opencode/rules/command-query-pattern.md)**
   - Commands are `public static class` with `HandleAsync` method
   - Commands use entity-specific repositories (`IEntityRepository`)
   - Queries use `DbContext` directly for LINQ composition
   - Return types: `Task<Result>`/`Task<Result<T>>` for commands, `ValueTask<Result<TResponse>>` for queries

4. **[Result Pattern](.opencode/rules/result-pattern.md)**
   - Use `Result` and `Result<T>` instead of throwing exceptions for normal flow
   - `Result` has `IsSuccess` flag and `ErrorMessages` array
   - `Result<T>` adds `Data` property for typed responses
   - Reserve exceptions for truly exceptional situations only

5. **[Repository Pattern](.opencode/rules/repository-pattern.md)**
   - Entity-specific repository interfaces (e.g., `IEntityRepository`)
   - Simple methods: `Add`, `Update`, `Remove`, `GetByIdAsync`, `SaveChangesAsync`
   - Used in commands for testability
   - NOT used in queries (use DbContext instead)

6. **[Entity Configuration Pattern](.opencode/rules/entity-configuration-pattern.md)**
   - Entities extend `BaseEntity<TKey>` (sealed records)
   - Nested `internal class Configuration : BaseConfiguration<TEntity>`
   - GuidV7 value generator for time-ordered GUIDs
   - Automatic timestamp tracking via `ITimeTrackable`

7. **[Testing Patterns](.opencode/rules/testing-patterns.md)**
   - xUnit with FluentAssertions and NSubstitute
   - Only commands are unit tested (mock repositories)
   - Queries are NOT unit tested (integration test in staging instead)
   - Test naming: `HandleAsync_Should{Behavior}_When{Condition}`

## Code Style Conventions

See **[Code Style Conventions](.opencode/rules/code-style-conventions.md)** for complete details.

### Key Conventions

#### Language Features
- **File-scoped namespaces**: Always use `namespace X.Y.Z;`
- **Implicit usings**: Enabled (do NOT add `using System;` etc.)
- **Nullable reference types**: Enabled (use `?` for nullable, `required` for mandatory)
- **var**: Preferred everywhere
- **Braces**: Always required
- **this. qualification**: Required for fields, methods, and properties

#### Naming Conventions
- **Interfaces**: `IApiEndpoint`, `IRepository`, `IValidator<T>`
- **Classes/Records**: `CreateEntityRequest`, `Entity`
- **Methods**: `HandleAsync`, `AddAsync`
- **Properties**: `Name`, `IsActive`
- **Local variables**: `camelCase` (e.g., `request`, `cancellationToken`)
- **Private fields**: `camelCase` with `this.` prefix, **NO underscore** (e.g., `this.dbContext`)
- **Constants**: `PascalCase` in static classes (e.g., `ApiVersions.V1`)

#### Type Conventions
- **DTOs**: `public sealed record`
- **Contract helper DTOs**: Top-level records in the same `.cs` file (avoid nested contract DTO types)
- **Entities**: `public sealed record` extending `BaseEntity<TKey>`
- **Endpoint Parameters**: `internal readonly struct` (or `record` if nullable)
- **Endpoint Services**: `internal readonly struct`
- **Commands/Queries**: `public static class`
- **Endpoint return**: `ValueTask<T>`

#### Access Modifiers
- **Endpoints**: `public class`
- **Endpoint Parameters/Services/Validators**: `internal`
- **Commands/Queries**: `public static class`
- **DTOs**: `public sealed record`
- **Entities**: `public sealed record`
- **Repository Interface**: `public interface`
- **Repository Implementation**: `public sealed class`

#### Import Ordering
1. Framework (`System.*`, `Microsoft.*`)
2. External (third-party packages)
3. Internal (project namespaces)

Sort alphabetically within each group.

#### CancellationToken
Always pass and propagate `CancellationToken`:
- In endpoints: Capture from `HttpContext.RequestAborted` via Services struct
- In commands/queries: Pass as last parameter
- Always propagate to async calls

## Common Workflows

### Adding a New Entity

1. Create entity in `src/BetterTracker.Data/Entities/{EntityName}Entity.cs`
2. Create repository interface `I{EntityName}Repository` in same file
3. Create repository implementation `{EntityName}Repository` in same file
4. Register repository in `Setup.cs`: `services.AddScoped<I{EntityName}Repository, {EntityName}Repository>()`
5. Add DbSet property in `AppDbContext.cs`
6. Create migration: `dotnet ef migrations add Add{EntityName}Entity --project src/BetterTracker.Data.Migrations --startup-project src/BetterTracker`
7. Update database: `dotnet ef database update --project src/BetterTracker.Data.Migrations --startup-project src/BetterTracker`

### Adding a New Command

1. Create request DTO in `src/BetterTracker.Contracts/{Domain}/`
2. Create command in `src/BetterTracker.Core/{Domain}/Commands/{CommandName}.cs`
3. Create endpoint in `src/BetterTracker.Api/{Domain}/Endpoints/{CommandName}Endpoint.cs`
4. Create test in `tests/BetterTracker.Tests/Commands/{CommandName}Tests.cs`
5. Run tests: `dotnet test --filter "FullyQualifiedName~{CommandName}Tests"`

### Adding a New Query

1. Create response DTO in `src/BetterTracker.Contracts/{Domain}/`
2. Create query in `src/BetterTracker.Core/{Domain}/Queries/{QueryName}.cs`
3. Create endpoint in `src/BetterTracker.Api/{Domain}/Endpoints/{QueryName}Endpoint.cs`
4. Test manually or via integration tests (do NOT unit test queries)

## Important Notes

- **Endpoints are auto-discovered** via reflection scanning for `IApiEndpoint` implementations
- **Entity configurations are auto-discovered** via `ApplyConfigurationsFromAssembly`
- **Validators are auto-discovered** when registered with `includeInternalTypes: true`
- **Never use in-memory databases** for testing queries (anti-pattern)
- **Never skip hooks** in git commands unless explicitly requested
- **Always use entity-specific repositories** in commands, never generic repositories
- **Always use DbContext directly** in queries, never repositories
- **Contracts should avoid nested DTO records** to prevent OpenAPI schema naming collisions
- **Contracts should be organized by feature/domain directories** (for example `Auth`, `JobApplications`, `Notes`, `Tags`, `Common`)

## Project Structure

```
backend/
├── src/
│   ├── BetterTracker/              # Composition root (Program.cs)
│   ├── BetterTracker.Api/          # Endpoints, validation, OpenAPI
│   ├── BetterTracker.Core/         # Commands and queries
│   ├── BetterTracker.Contracts/    # Request/Response DTOs organized by feature/domain
│   ├── BetterTracker.Data/         # DbContext, entities, repositories
│   ├── BetterTracker.Data.Migrations/  # EF Core migrations
│   └── BetterTracker.Common/       # Shared utilities, constants
└── tests/
    └── BetterTracker.Tests/        # xUnit tests
```

## Additional Resources

All detailed architectural patterns and conventions are documented in `.opencode/rules/`:
- `clean-architecture-structure.md` - Solution structure and layer responsibilities
- `endpoint-patterns.md` - How to create API endpoints
- `command-query-pattern.md` - Command and query implementation patterns
- `result-pattern.md` - Result types for error handling without exceptions
- `repository-pattern.md` - Entity-specific repository pattern
- `entity-configuration-pattern.md` - Entity and EF Core configuration
- `testing-patterns.md` - xUnit testing with mocks
- `code-style-conventions.md` - Comprehensive style guide

**Always consult these files before implementing new features or making architectural decisions.**
