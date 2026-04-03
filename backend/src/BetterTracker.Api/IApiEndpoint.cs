using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace BetterTracker.Api;

internal interface IApiEndpoint
{
    ApiVersion Version { get; }
    string DefaultTag { get; }
    IEndpointConventionBuilder Register(IEndpointRouteBuilder builder);
}
