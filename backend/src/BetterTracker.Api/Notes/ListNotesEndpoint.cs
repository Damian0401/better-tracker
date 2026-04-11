using System.Security.Claims;
using Asp.Versioning;
using BetterTracker.Common.Helpers;
using BetterTracker.Contracts;
using BetterTracker.Core.Notes.Queries;
using BetterTracker.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BetterTracker.Api.Notes;

public class ListNotesEndpoint : IApiEndpoint
{
    public ApiVersion Version => ApiVersions.V1;
    public string DefaultTag => ApiTags.Notes;

    public IEndpointConventionBuilder Register(IEndpointRouteBuilder builder) =>
        builder.MapGet("/notes", HandleAsync).RequireAuthorization();

    private static async ValueTask<Results<Ok<ListNotesResponse>, UnauthorizedHttpResult>> HandleAsync(
        [AsParameters] Parameters parameters,
        [AsParameters] Services services)
    {
        var userIdResult = UserIdHelper.GetUserId(services.HttpContextAccessor.HttpContext!);
        if (!userIdResult.IsSuccess)
        {
            return TypedResults.Unauthorized();
        }

        var response = await ListNotes.HandleAsync(
            parameters.Count,
            parameters.Skip,
            userIdResult.Data,
            services.DbContext,
            services.CancellationToken);

        return TypedResults.Ok(response);
    }

    internal record Parameters
    {
        [FromQuery]
        public int? Count { get; init; }

        [FromQuery]
        public int? Skip { get; init; }
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
