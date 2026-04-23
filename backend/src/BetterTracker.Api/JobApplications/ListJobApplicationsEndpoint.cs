using Asp.Versioning;
using BetterTracker.Common.Helpers;
using BetterTracker.Contracts;
using BetterTracker.Core.JobApplications.Queries;
using BetterTracker.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BetterTracker.Api.JobApplications;

public class ListJobApplicationsEndpoint : IApiEndpoint
{
    public ApiVersion Version => ApiVersions.V1;
    public string DefaultTag => ApiTags.JobApplications;

    public IEndpointConventionBuilder Register(IEndpointRouteBuilder builder) =>
        builder.MapGet("/job-applications", HandleAsync).RequireAuthorization();

    private static async ValueTask<Results<Ok<ListJobApplicationsResponse>, UnauthorizedHttpResult>> HandleAsync(
        [AsParameters] Parameters parameters,
        [AsParameters] Services services)
    {
        var userIdResult = UserIdHelper.GetUserId(services.HttpContextAccessor.HttpContext!);
        if (!userIdResult.IsSuccess)
        {
            return TypedResults.Unauthorized();
        }

        var response = await ListJobApplications.HandleAsync(
            parameters.Count,
            parameters.Skip,
            parameters.Statuses,
            parameters.Tags,
            parameters.WorkTypes,
            parameters.Search,
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

        [FromQuery]
        public int[]? Statuses { get; init; }

        [FromQuery]
        public string[]? Tags { get; init; }

        [FromQuery]
        public int[]? WorkTypes { get; init; }

        [FromQuery]
        public string? Search { get; init; }
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
