using Asp.Versioning;
using BetterTracker.Common.Helpers;
using BetterTracker.Contracts;
using BetterTracker.Core.JobApplications.Commands;
using BetterTracker.Data.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BetterTracker.Api.JobApplications;

public class AddJobApplicationCommentEndpoint : IApiEndpoint
{
    public ApiVersion Version => ApiVersions.V1;
    public string DefaultTag => ApiTags.JobApplications;

    public IEndpointConventionBuilder Register(IEndpointRouteBuilder builder) =>
        builder.MapPost("/comments", HandleAsync).WithValidation<Parameters>().RequireAuthorization();

    private static async ValueTask<Results<Created<AddJobApplicationCommentResponse>, NotFound, UnauthorizedHttpResult>> HandleAsync(
        [AsParameters] Parameters parameters,
        [AsParameters] Services services)
    {
        var userIdResult = UserIdHelper.GetUserId(services.HttpContextAccessor.HttpContext!);
        if (!userIdResult.IsSuccess)
        {
            return TypedResults.Unauthorized();
        }

        var result = await AddJobApplicationComment.HandleAsync(
            parameters.Request,
            userIdResult.Data,
            services.JobApplicationRepository,
            services.CancellationToken);

        if (!result.IsSuccess)
        {
            return TypedResults.NotFound();
        }

        var locationUri = $"/comments/{result.Data!.Id}";
        return TypedResults.Created(locationUri, result.Data!);
    }

    internal class Validator : AbstractValidator<Parameters>
    {
        public Validator()
        {
            this.RuleFor(x => x.Request.Content)
                .NotEmpty()
                .MaximumLength(2000);

            this.RuleFor(x => x.Request.JobApplicationId)
                .NotEmpty();
        }
    }

    internal readonly struct Parameters
    {
        [FromBody]
        public required AddJobApplicationCommentRequest Request { get; init; }
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
