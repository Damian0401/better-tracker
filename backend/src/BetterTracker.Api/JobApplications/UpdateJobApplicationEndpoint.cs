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

public class UpdateJobApplicationEndpoint : IApiEndpoint
{
    public ApiVersion Version => ApiVersions.V1;
    public string DefaultTag => ApiTags.JobApplications;

    public IEndpointConventionBuilder Register(IEndpointRouteBuilder builder) =>
        builder.MapPut("/job-applications/{id:guid}", HandleAsync).WithValidation<Parameters>().RequireAuthorization();

    private static async ValueTask<Results<NoContent, NotFound, BadRequest<ErrorResponse>, UnauthorizedHttpResult>> HandleAsync(
        [AsParameters] Parameters parameters,
        [AsParameters] Services services)
    {
        var userIdResult = UserIdHelper.GetUserId(services.HttpContextAccessor.HttpContext!);
        if (!userIdResult.IsSuccess)
        {
            return TypedResults.Unauthorized();
        }

        var request = new UpdateJobApplicationRequest
        {
            Id = parameters.Id,
            Title = parameters.Request.Title,
            JobTitle = parameters.Request.JobTitle,
            Description = parameters.Request.Description,
            CompanyName = parameters.Request.CompanyName,
            Requirements = parameters.Request.Requirements,
            Benefits = parameters.Request.Benefits,
            Link = parameters.Request.Link,
            Technologies = parameters.Request.Technologies,
            Experience = parameters.Request.Experience,
            WorkType = parameters.Request.WorkType,
            CurrentStatus = parameters.Request.CurrentStatus,
            Salaries = parameters.Request.Salaries,
            Tags = parameters.Request.Tags,
        };

        var result = await UpdateJobApplication.HandleAsync(
            request,
            userIdResult.Data,
            services.JobApplicationRepository,
            services.TagRepository,
            services.CancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorMessages.Contains("Job application not found"))
            {
                return TypedResults.NotFound();
            }

            return TypedResults.BadRequest(new ErrorResponse
            {
                Errors = result.ErrorMessages,
            });
        }

        return TypedResults.NoContent();
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
        [FromRoute]
        public required Guid Id { get; init; }

        [FromBody]
        public required UpdateJobApplicationBody Request { get; init; }
    }

    internal sealed record UpdateJobApplicationBody
    {
        public required string Title { get; init; }
        public required string JobTitle { get; init; }
        public string? Description { get; init; }
        public required string CompanyName { get; init; }
        public string? Requirements { get; init; }
        public string? Benefits { get; init; }
        public string? Link { get; init; }
        public string? Technologies { get; init; }
        public string? Experience { get; init; }
        public required int WorkType { get; init; }
        public required int CurrentStatus { get; init; }
        public IReadOnlyList<UpdateJobApplicationRequest.SalaryDto>? Salaries { get; init; }
        public IReadOnlyList<string>? Tags { get; init; }
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
