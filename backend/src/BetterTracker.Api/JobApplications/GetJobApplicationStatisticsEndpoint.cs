using Asp.Versioning;
using BetterTracker.Common.Helpers;
using BetterTracker.Contracts;
using BetterTracker.Core.JobApplications.Queries;
using BetterTracker.Data;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BetterTracker.Api.JobApplications;

public class GetJobApplicationStatisticsEndpoint : IApiEndpoint
{
    public ApiVersion Version => ApiVersions.V1;
    public string DefaultTag => ApiTags.JobApplications;

    public IEndpointConventionBuilder Register(IEndpointRouteBuilder builder) =>
        builder.MapGet("/job-applications/statistics", HandleAsync).WithValidation<Parameters>().RequireAuthorization();

    private static async ValueTask<Results<Ok<GetJobApplicationStatisticsResponse>, UnauthorizedHttpResult>> HandleAsync(
        [AsParameters] Parameters parameters,
        [AsParameters] Services services)
    {
        var userIdResult = UserIdHelper.GetUserId(services.HttpContextAccessor.HttpContext!);
        if (!userIdResult.IsSuccess)
        {
            return TypedResults.Unauthorized();
        }

        var response = await GetJobApplicationStatistics.HandleAsync(
            parameters.DateFrom,
            parameters.DateTo,
            parameters.IncludeArchived,
            userIdResult.Data,
            services.DbContext,
            services.CancellationToken);

        return TypedResults.Ok(response);
    }

    internal class Validator : AbstractValidator<Parameters>
    {
        public Validator()
        {
            this.RuleFor(x => x)
                .Must(x => !x.DateFrom.HasValue || !x.DateTo.HasValue || x.DateFrom.Value <= x.DateTo.Value)
                .WithMessage("dateFrom must be less than or equal to dateTo.");
        }
    }

    internal record Parameters
    {
        [FromQuery]
        public DateTimeOffset? DateFrom { get; init; }

        [FromQuery]
        public DateTimeOffset? DateTo { get; init; }

        [FromQuery]
        public bool? IncludeArchived { get; init; }
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
