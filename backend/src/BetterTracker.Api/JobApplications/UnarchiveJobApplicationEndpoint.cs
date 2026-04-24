using Asp.Versioning;
using BetterTracker.Common.Helpers;
using BetterTracker.Contracts;
using BetterTracker.Core.JobApplications.Commands;
using BetterTracker.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BetterTracker.Api.JobApplications;

public class UnarchiveJobApplicationEndpoint : IApiEndpoint
{
    public ApiVersion Version => ApiVersions.V1;
    public string DefaultTag => ApiTags.JobApplications;

    public IEndpointConventionBuilder Register(IEndpointRouteBuilder builder) =>
        builder.MapPatch("/job-applications/{id:guid}/unarchive", HandleAsync).RequireAuthorization();

    private static async ValueTask<Results<NoContent, NotFound, UnauthorizedHttpResult>> HandleAsync(
        [AsParameters] Parameters parameters,
        [AsParameters] Services services)
    {
        var userIdResult = UserIdHelper.GetUserId(services.HttpContextAccessor.HttpContext!);
        if (!userIdResult.IsSuccess)
        {
            return TypedResults.Unauthorized();
        }

        var result = await UnarchiveJobApplication.HandleAsync(
            new UnarchiveJobApplicationRequest
            {
                Id = parameters.Id,
            },
            userIdResult.Data,
            services.JobApplicationRepository,
            services.CancellationToken);

        if (!result.IsSuccess)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.NoContent();
    }

    internal readonly struct Parameters
    {
        [FromRoute]
        public required Guid Id { get; init; }
    }

    internal readonly struct Services
    {
        [FromServices]
        public required IJobApplicationRepository JobApplicationRepository { get; init; }

        [FromServices]
        public required IHttpContextAccessor HttpContextAccessor { get; init; }

        public required CancellationToken CancellationToken { get; init; }
    }
}
