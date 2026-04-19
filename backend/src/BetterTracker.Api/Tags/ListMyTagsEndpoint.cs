using Asp.Versioning;
using BetterTracker.Common.Helpers;
using BetterTracker.Contracts;
using BetterTracker.Core.Tags.Queries;
using BetterTracker.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BetterTracker.Api.Tags;

public class ListMyTagsEndpoint : IApiEndpoint
{
    public ApiVersion Version => ApiVersions.V1;
    public string DefaultTag => ApiTags.Tags;

    public IEndpointConventionBuilder Register(IEndpointRouteBuilder builder) =>
        builder.MapGet("/tags/me", HandleAsync).RequireAuthorization();

    private static async ValueTask<Results<Ok<ListMyTagsResponse>, UnauthorizedHttpResult>> HandleAsync(
        [AsParameters] Services services)
    {
        var userIdResult = UserIdHelper.GetUserId(services.HttpContextAccessor.HttpContext!);
        if (!userIdResult.IsSuccess)
        {
            return TypedResults.Unauthorized();
        }

        var response = await ListMyTags.HandleAsync(
            userIdResult.Data,
            services.DbContext,
            services.CancellationToken);

        return TypedResults.Ok(response);
    }

    internal readonly struct Services
    {
        [FromServices]
        public required AppDbContext DbContext { get; init; }

        [FromServices]
        public required IHttpContextAccessor HttpContextAccessor { get; init; }

        public required CancellationToken CancellationToken { get; init; }
    }
}
