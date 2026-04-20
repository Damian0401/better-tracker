# Code Style Conventions

## C# Language Features

| Feature | Setting |
|---|---|
| File-scoped namespaces | Required (`namespace X.Y.Z;`) |
| Implicit usings | Enabled — do not add `using System;` etc. |
| Nullable reference types | Enabled — use `?` for nullable, `required` for mandatory init properties |
| `var` usage | Preferred everywhere |
| Braces | Always required (`csharp_prefer_braces = true`) |
| UTF-8 string literals | Preferred |
| Trailing commas | Enabled in multiline lists |
| Object/collection initializers | Chop always (each element on its own line) |

## Formatting

| Rule | Setting |
|---|---|
| Encoding | UTF-8 |
| Line endings | CRLF |
| Indentation | 4 spaces |

## Qualification

| Rule | Setting |
|---|---|
| Fields | `this.` required (`dotnet_style_qualification_for_field = true`) |
| Methods | `this.` required (`dotnet_style_qualification_for_method = true`) |
| Properties | `this.` required (`dotnet_style_qualification_for_property = true`) |

## Modifier Order

`public, private, protected, internal, file, new, static, abstract, virtual, sealed, readonly, override, extern, unsafe, volatile, async, required`

## Naming Conventions

| Element | Convention | Example |
|---|---|---|
| Interfaces | `I` + PascalCase | `IApiEndpoint`, `IValidator<T>`, `IRepository` |
| Classes/Records/Structs | PascalCase | `CreateEntityRequest`, `Entity` |
| Static classes | PascalCase | `CreateEntity`, `ApiVersions` |
| Methods | PascalCase | `HandleAsync`, `AddAsync` |
| Properties | PascalCase | `Name`, `IsActive` |
| Local variables / parameters | camelCase | `request`, `cancellationToken` |
| Private fields | camelCase, **no** underscore prefix | `this.dbContext` |
| Constants | PascalCase in static classes | `ApiVersions.V1`, `ApiTags.Default` |
| Files | Named after the primary type | `CreateEntityEndpoint.cs` |

## Type Conventions

| Scenario | Type Choice |
|---|---|
| DTOs | `sealed record` |
| Contract helper DTOs | Top-level `sealed record` in the same file (not nested) |
| Entities | `sealed record` extending `BaseEntity<TKey>` |
| Endpoint Parameters | `readonly struct` (or `record` if nullable properties) |
| Endpoint Services | `readonly struct` |
| Commands/Queries | `static class` |
| Endpoint return | `ValueTask<T>` |
| Command return | `Task` or `Task<T>` |

## Access Modifier Conventions

| Element | Modifier |
|---|---|
| Endpoints | `public class` |
| Endpoint Parameters/Services/Validators | `internal` |
| Commands/Queries | `public static class` |
| DTOs | `public sealed record` |
| Entities | `public sealed record` |
| Entity Configurations | `internal class` (nested) |
| Repository Interface | `public interface` |
| Repository Implementation | `public sealed class` |
| DbContext | `public sealed class` |
| Constants classes | `internal static class` (API) / `public static class` (Database) |
| Value generators | `internal sealed class` |
| Sentinel Assembly class | `internal sealed class` |

## Import Ordering

Group `using` directives alphabetically:
1. Framework (System.*, Microsoft.*)
2. External (third-party packages)
3. Internal (project namespaces)

Within each group, sort alphabetically.

## CancellationToken

Always pass `CancellationToken` and propagate to async calls:

```csharp
public static async Task HandleAsync(
    CreateEntityRequest request,
    IRepository repository,
    CancellationToken cancellationToken)
{
    // ...
    await repository.SaveChangesAsync(cancellationToken);
}
```

In endpoints, capture from `HttpContext.RequestAborted`:

```csharp
internal readonly struct Services
{
    [FromServices]
    public required IRepository Repository { get; init; }

    [FromServices]
    public required AppDbContext DbContext { get; init; }

    public required CancellationToken CancellationToken { get; init; }
}
```
