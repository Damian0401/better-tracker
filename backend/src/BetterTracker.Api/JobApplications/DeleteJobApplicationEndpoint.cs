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

public class DeleteJobApplicationEndpoint : IApiEndpoint
{
    public ApiVersion Version => ApiVersions.V1;
    public string DefaultTag => ApiTags.JobApplications;

    public IEndpointConventionBuilder Register(IEndpointRouteBuilder builder) =>
        builder.MapDelete("/job-applications/{id:guid}", HandleAsync).RequireAuthorization();

    private static async ValueTask<Results<NoContent, NotFound, UnauthorizedHttpResult>> HandleAsync(
        [AsParameters] Parameters parameters,
        [AsParameters] Services services)
    {
        var userIdResult = UserIdHelper.GetUserId(services.HttpContextAccessor.HttpContext!);
        if (!userIdResult.IsSuccess)
        {
            return TypedResults.Unauthorized();
        }

        var request = new DeleteJobApplicationRequest
        {
            Id = parameters.Id,
        };

        var result = await DeleteJobApplication.HandleAsync(
            request,
            userIdResult.Data,
            services.JobApplicationRepository,
            services.TagRepository,
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
        public required ITagRepository TagRepository { get; init; }

        [FromServices]
        public required IHttpContextAccessor HttpContextAccessor { get; init; }

        public required CancellationToken CancellationToken { get; init; }
    }
}
