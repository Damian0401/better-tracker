# Clean Architecture Structure

## Overview

.NET 9.0 ASP.NET Core Web API using a clean architecture with minimal endpoints and EF Core.

## Directory Structure

All projects live inside the `src/` directory. Tests live inside the `tests/` directory.

```
solution-root/
  src/
    {Root}/                    # Composition root, Program.cs
    {Root}.Api/                # Endpoints, validation, OpenAPI
    {Root}.Core/               # Business logic (commands/queries)
    {Root}.Contracts/          # Request/Response DTOs organized by feature/domain
    {Root}.Data/               # DbContext, entities, configurations, repositories
    {Root}.Data.Migrations/    # EF Core migrations
    {Root}.Common/             # Shared utilities, constants
  tests/
    {Root}.Tests/              # xUnit test project
```

## Solution Structure

| Project | SDK | Role |
|---|---|---|
| `src/{Root}` | `Microsoft.NET.Sdk.Web` | Composition root, `Program.cs` |
| `src/{Root}.Api` | `Microsoft.NET.Sdk` | Endpoints, validation, OpenAPI, API versioning |
| `src/{Root}.Core` | `Microsoft.NET.Sdk` | Business logic (commands/queries) |
| `src/{Root}.Contracts` | `Microsoft.NET.Sdk` | Request/Response DTOs organized by feature/domain |
| `src/{Root}.Data` | `Microsoft.NET.Sdk` | DbContext, entities, configurations, repositories |
| `src/{Root}.Data.Migrations` | `Microsoft.NET.Sdk` | EF Core migrations (design-time only) |
| `src/{Root}.Common` | `Microsoft.NET.Sdk` | Shared utilities, constants, TimeProvider registration |

## Build / Run Commands

```bash
# Build entire solution
dotnet build

# Run the application
dotnet run --project src/{Root}

# Add a migration
dotnet ef migrations add <Name> --project src/{Root}.Data.Migrations --startup-project src/{Root}

# Update database
dotnet ef database update --project src/{Root}.Data.Migrations --startup-project src/{Root}

# Run all tests
dotnet test tests/

# Run a single test by fully qualified name
dotnet test --filter "FullyQualifiedName~YourTestName"
```

## Dependency Graph

```
src/{Root} (Web entry point)
  |-- src/{Root}.Api (Endpoints, validation, OpenAPI)
  |     |-- src/{Root}.Core (Commands/Queries)
  |     |     |-- src/{Root}.Contracts (DTOs by domain)
  |     |     |     |-- src/{Root}.Data (Entities, DbContext, Repository)
  |     |     |-- src/{Root}.Data
  |     |-- src/{Root}.Data
  |-- src/{Root}.Data
  |-- src/{Root}.Common (shared utilities)

tests/{Root}.Tests
  |-- {Root}.Core
  |-- {Root}.Contracts
  |-- {Root}.Data
```

Key decisions:
- `Data` project houses DbContext, entities, configurations, and `IRepository` + `EfCoreRepository`.
- `Data.Migrations` is **design-time only** — not referenced by any runtime project.
- All projects reference `Data` directly for entities, context, and repository.

## Layer Responsibilities

### Composition Root (`{Root}`)
- Entry point (`Program.cs`)
- Wires together all layers via extension methods
- No business logic

### Presentation (`{Root}.Api`)
- Endpoint definitions implementing `IApiEndpoint`
- Validation filters (FluentValidation integration)
- OpenAPI + Scalar documentation setup
- API versioning configuration
- Auto-discovers endpoints via reflection

### Application (`{Root}.Core`)
- Commands and queries as static classes with `HandleAsync` methods
- Commands injected with `IRepository` for mutations
- Queries injected with `DbContext` for composable LINQ
- Depends on Contracts (DTOs) and Data (entities, context, repository)

### Contracts (`{Root}.Contracts`)
- Sealed records for request/response DTOs
- Organize DTO files by feature/domain folders (for example `Auth`, `JobApplications`, `Notes`, `Tags`, `Common`)
- Use top-level DTO records; helper DTOs stay in the same `.cs` file but are not nested

### Domain/Infrastructure (`{Root}.Data`)
- EF Core DbContext with automatic timestamp tracking
- Entity definitions as sealed records
- Nested entity configurations
- Auto-discovery of configurations via reflection
- `IRepository` interface and `EfCoreRepository` implementation

### Shared (`{Root}.Common`)
- Constants (schemas, config keys)
- TimeProvider registration
- Utility extensions

## Setup Extension Method Pattern

Each layer exposes setup via extension methods following this convention:

| Project | Add Method | Use Method | Map Method |
|---|---|---|---|
| `Common` | `AddCommon(this IHostApplicationBuilder)` | — | — |
| `Data` | `AddDatabase(this IHostApplicationBuilder)` | — | — |
| `Data` | `AddRepository(this IServiceCollection)` | — | — |
| `Api` | `AddApi(this IHostApplicationBuilder)` | `UseApi(this IApplicationBuilder)` | `MapApi(this IEndpointRouteBuilder)` |

### Program.cs Template

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.AddCommon();
builder.AddDatabase();
builder.Services.AddRepository();
builder.AddApi();

var app = builder.Build();
app.UseApi();
app.MapApi();

app.Run();
```

### Setup Class Template

```csharp
public static class Setup
{
    public static IHostApplicationBuilder AddLayer(this IHostApplicationBuilder builder)
    {
        // Register services
        builder.Services.AddSingleton(...);
        return builder;
    }

    public static IApplicationBuilder UseLayer(this IApplicationBuilder builder)
    {
        // Configure middleware
        return builder;
    }

    public static IEndpointRouteBuilder MapLayer(this IEndpointRouteBuilder builder)
    {
        // Map endpoints
        return builder;
    }
}
```

## Key Third-Party Packages

| Package | Used In |
|---|---|
| `Asp.Versioning.Http` | Api |
| `Asp.Versioning.Mvc.ApiExplorer` | Api |
| `FluentValidation` | Api |
| `FluentValidation.DependencyInjectionExtensions` | Api |
| `Microsoft.AspNetCore.OpenApi` | Api |
| `Scalar.AspNetCore` | Api |
| `Microsoft.EntityFrameworkCore.Sqlite` | Data |
