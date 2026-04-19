using Asp.Versioning;
using BetterTracker.Contracts;
using BetterTracker.Core.JobApplications.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace BetterTracker.Api.JobApplications;

public class GetJobApplicationDropdownsEndpoint : IApiEndpoint
{
    public ApiVersion Version => ApiVersions.V1;
    public string DefaultTag => ApiTags.JobApplications;

    public IEndpointConventionBuilder Register(IEndpointRouteBuilder builder) =>
        builder.MapGet("/job-applications/dropdowns", HandleAsync).RequireAuthorization();

    private static async ValueTask<Ok<GetJobApplicationDropdownsResponse>> HandleAsync()
    {
        var response = await GetJobApplicationDropdowns.HandleAsync();
        return TypedResults.Ok(response);
    }
}
