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

public class GetJobApplicationByIdEndpoint : IApiEndpoint
{
    public ApiVersion Version => ApiVersions.V1;
    public string DefaultTag => ApiTags.JobApplications;

    public IEndpointConventionBuilder Register(IEndpointRouteBuilder builder) =>
        builder.MapGet("/job-applications/{id:guid}", HandleAsync).RequireAuthorization();

    private static async ValueTask<Results<Ok<GetJobApplicationByIdResponse>, NotFound, UnauthorizedHttpResult>> HandleAsync(
        [AsParameters] Parameters parameters,
        [AsParameters] Services services)
    {
        var userIdResult = UserIdHelper.GetUserId(services.HttpContextAccessor.HttpContext!);
        if (!userIdResult.IsSuccess)
        {
            return TypedResults.Unauthorized();
        }

        var response = await GetJobApplicationById.HandleAsync(
            parameters.Id,
            userIdResult.Data,
            services.DbContext,
            services.CancellationToken);

        if (response is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(response);
    }

    internal readonly struct Parameters
    {
        [FromRoute]
        public required Guid Id { get; init; }
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
