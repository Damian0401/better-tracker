using Asp.Versioning;
using Asp.Versioning.Builder;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace BetterTracker.Api;

public static class Setup
{
    public static IHostApplicationBuilder AddApi(this IHostApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        builder.Services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = ApiVersions.V1;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

        builder.Services.AddOpenApi();

        ValidatorOptions.Global.LanguageManager.Culture = new("en");
        builder.Services.AddValidatorsFromAssemblyContaining<Assembly>(includeInternalTypes: true);

        builder.Services.AddApiEndpointsFromAssembly<Assembly>();

        return builder;
    }

    public static WebApplication UseApi(this WebApplication app)
    {
        app.UseCors();
        app.MapOpenApi();
        app.MapScalarApiReference();
        return app;
    }

    public static IEndpointRouteBuilder MapApi(this IEndpointRouteBuilder builder)
    {
        ApiVersionSet apiVersionSet = builder.NewApiVersionSet()
            .HasApiVersion(ApiVersions.V1)
            .ReportApiVersions()
            .Build();

        var group = builder
            .MapGroup("/api/v{version:apiVersion}")
            .WithApiVersionSet(apiVersionSet);

        builder.MapApiEndpoints(group);

        return builder;
    }
}
