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
            parameters.Status,
            parameters.Tag,
            parameters.WorkType,
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
        public int? Status { get; init; }

        [FromQuery]
        public string? Tag { get; init; }

        [FromQuery]
        public int? WorkType { get; init; }

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
