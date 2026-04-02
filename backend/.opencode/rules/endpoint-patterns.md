# Endpoint Patterns

## Overview

Endpoints are classes implementing `IApiEndpoint` with automatic discovery via reflection. Each endpoint encapsulates its route, handler, parameters, services, and validator in a single file.

## IApiEndpoint Interface

```csharp
internal interface IApiEndpoint
{
    ApiVersion Version { get; }
    string DefaultTag { get; }
    IEndpointConventionBuilder Register(IEndpointRouteBuilder builder);
}
```

The interface is `internal` — endpoints are discovered by assembly scanning, not registered manually.

## Complete Endpoint Anatomy

```csharp
public class CreateEntityEndpoint : IApiEndpoint
{
    // 1. Version and tag
    public ApiVersion Version => ApiVersions.V1;
    public string DefaultTag => ApiTags.Default;

    // 2. Route registration with validation
    public IEndpointConventionBuilder Register(IEndpointRouteBuilder builder) =>
        builder.MapPost("/entities", HandleAsync).WithValidation<Parameters>();

    // 3. Handler
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

    // 4. Validator (nested, internal)
    internal class Validator : AbstractValidator<Parameters>
    {
        public Validator()
        {
            this.RuleFor(x => x.Request.Name)
                .NotEmpty()
                .Length(3, 100);
        }
    }

    // 5. Parameters (nested, internal)
    internal readonly struct Parameters
    {
        [FromBody]
        public CreateEntityRequest Request { get; init; }
    }

    // 6. Services (nested, internal)
    internal readonly struct Services
    {
        [FromServices]
        public required IRepository Repository { get; init; }

        public required CancellationToken CancellationToken { get; init; }
    }
}
```

## Parameters Pattern

Use `readonly struct` when all properties are value types or simple references. Use `record` when nullable reference types are needed.

```csharp
// Struct — all simple types
internal readonly struct Parameters
{
    [FromBody]
    public CreateEntityRequest Request { get; init; }
}

// Record — nullable properties
internal record Parameters
{
    [FromQuery]
    public int? Count { get; init; }
}
```

Binding attributes go on properties: `[FromBody]`, `[FromQuery]`, `[FromServices]`. Properties use `init` accessors.

## Services Pattern

Always a `readonly struct` with `required` properties. `[FromServices]` on DI-injected dependencies. `CancellationToken` is captured automatically from `HttpContext.RequestAborted` — no attribute needed.

```csharp
internal readonly struct Services
{
    [FromServices]
    public required IRepository Repository { get; init; }

    [FromServices]
    public required AppDbContext DbContext { get; init; }

    [FromServices]
    public required TimeProvider TimeProvider { get; init; }

    public required CancellationToken CancellationToken { get; init; }
}
```

## Automatic Endpoint Discovery

### Registration (at startup)

```csharp
internal static IServiceCollection AddApiEndpointsFromAssembly<TAssembly>(
    this IServiceCollection services,
    ServiceLifetime lifetime = ServiceLifetime.Singleton)
{
    var types = typeof(TAssembly)
        .Assembly
        .DefinedTypes
        .Where(x => x is { IsAbstract: false, IsInterface: false } &&
                    x.IsAssignableTo(typeof(IApiEndpoint)))
        .Select(t => ServiceDescriptor.Describe(typeof(IApiEndpoint), t, lifetime));
    services.TryAddEnumerable(types);
    return services;
}
```

### Mapping (at routing time)

```csharp
internal static RouteGroupBuilder MapApiEndpoints(
    this IEndpointRouteBuilder builder,
    RouteGroupBuilder group)
{
    var endpoints = builder.ServiceProvider.GetServices<IApiEndpoint>();
    foreach (var endpoint in endpoints)
    {
        endpoint.Register(group)
            .HasApiVersion(endpoint.Version)
            .WithTags(endpoint.DefaultTag);
    }
    return group;
}
```

### Sentinel Assembly Class

A sentinel type is used purely as a type anchor for `typeof(TAssembly).Assembly` scanning:

```csharp
internal sealed class Assembly;
```

Usage: `builder.Services.AddApiEndpointsFromAssembly<Assembly>();`

## Validation Filter

The `.WithValidation<TType>()` extension adds an endpoint filter that:
1. Resolves `IValidator<TType>` from DI
2. Iterates endpoint arguments looking for one matching `TType`
3. Returns `TypedResults.ValidationProblem()` on failure
4. Proceeds to handler on success

```csharp
internal static IEndpointConventionBuilder WithValidation<TType>(
    this IEndpointConventionBuilder builder)
{
    builder.AddEndpointFilter(async static (context, next) =>
    {
        var services = context.HttpContext.RequestServices;
        var validator = services.GetRequiredService<IValidator<TType>>();
        var cancellationToken = context.HttpContext.RequestAborted;

        foreach (var argument in context.Arguments)
        {
            if (argument is not TType) continue;

            var validationContext = new ValidationContext<object>(argument);
            var validationResult = await validator.ValidateAsync(validationContext, cancellationToken);
            if (validationResult.IsValid) continue;

            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }
        return await next(context);
    });
    return builder;
}
```

Validators are registered automatically with `includeInternalTypes: true`:

```csharp
ValidatorOptions.Global.LanguageManager.Culture = new("en");
builder.Services.AddValidatorsFromAssemblyContaining<Assembly>(includeInternalTypes: true);
```

## API Versioning

Routes use the pattern `/api/v{version:apiVersion}`:

```csharp
var api = builder.NewVersionedApi();
var group = api.MapGroup("/api/v{version:apiVersion}");
builder.MapApiEndpoints(group);
```

## Endpoint Return Types

| Scenario | Return Type | Result |
|---|---|---|
| Command (no response body) | `ValueTask<NoContent>` | `TypedResults.NoContent()` |
| Command (with response body) | `ValueTask<Ok<TResponse>>` | `TypedResults.Ok(response)` |
| Query (with response body) | `ValueTask<Ok<TResponse>>` | `TypedResults.Ok(response)` |
| Validation failure | — | `TypedResults.ValidationProblem(...)` (via filter) |
| Unhandled exception | — | Global ProblemDetails middleware |
