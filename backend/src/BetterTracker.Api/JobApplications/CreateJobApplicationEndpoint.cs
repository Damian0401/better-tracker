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

public class CreateJobApplicationEndpoint : IApiEndpoint
{
    public ApiVersion Version => ApiVersions.V1;
    public string DefaultTag => ApiTags.JobApplications;

    public IEndpointConventionBuilder Register(IEndpointRouteBuilder builder) =>
        builder.MapPost("/job-applications", HandleAsync).WithValidation<Parameters>().RequireAuthorization();

    private static async ValueTask<Results<Created<CreateJobApplicationResponse>, BadRequest<ErrorResponse>, UnauthorizedHttpResult>> HandleAsync(
        [AsParameters] Parameters parameters,
        [AsParameters] Services services)
    {
        var userIdResult = UserIdHelper.GetUserId(services.HttpContextAccessor.HttpContext!);
        if (!userIdResult.IsSuccess)
        {
            return TypedResults.Unauthorized();
        }

        var result = await CreateJobApplication.HandleAsync(
            parameters.Request,
            userIdResult.Data,
            services.JobApplicationRepository,
            services.TagRepository,
            services.CancellationToken);

        if (!result.IsSuccess)
        {
            return TypedResults.BadRequest(new ErrorResponse
            {
                Errors = result.ErrorMessages,
            });
        }

        var locationUri = $"/job-applications/{result.Data!.Id}";

        return TypedResults.Created(locationUri, result.Data);
    }

    internal class Validator : AbstractValidator<Parameters>
    {
        public Validator()
        {
            this.RuleFor(x => x.Request.Title)
                .NotEmpty()
                .MaximumLength(200);

            this.RuleFor(x => x.Request.JobTitle)
                .NotEmpty()
                .MaximumLength(200);

            this.RuleFor(x => x.Request.CompanyName)
                .NotEmpty()
                .MaximumLength(200);

            this.RuleFor(x => x.Request.Link)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Request.Link));

            this.RuleForEach(x => x.Request.Tags)
                .MaximumLength(50)
                .When(x => x.Request.Tags is not null);

            this.RuleForEach(x => x.Request.Salaries)
                .ChildRules(salary =>
                {
                    salary.RuleFor(x => x.Currency)
                        .Length(3)
                        .When(x => !string.IsNullOrWhiteSpace(x.Currency));
                });
        }
    }

    internal readonly struct Parameters
    {
        [FromBody]
        public required CreateJobApplicationRequest Request { get; init; }
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
