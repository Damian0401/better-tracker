using Asp.Versioning;
using BetterTracker.Common;
using BetterTracker.Contracts;
using BetterTracker.Core.Auth.Commands;
using BetterTracker.Data.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BetterTracker.Api.Auth;

public class LoginEndpoint : IApiEndpoint
{
    public ApiVersion Version => ApiVersions.V1;
    public string DefaultTag => ApiTags.Auth;

    public IEndpointConventionBuilder Register(IEndpointRouteBuilder builder) =>
        builder.MapPost("/auth/login", HandleAsync).WithValidation<Parameters>();

    private static async ValueTask<Results<Ok<AuthResponse>, UnauthorizedHttpResult>> HandleAsync(
        [AsParameters] Parameters parameters,
        [AsParameters] Services services)
    {
        var result = await LoginUser.HandleAsync(
            parameters.Request,
            services.UserRepository,
            services.CancellationToken);

        if (!result.IsSuccess)
        {
            return TypedResults.Unauthorized();
        }

        var token = services.TokenService.GenerateToken(result.Data!.Id, result.Data.UserName);

        var response = new AuthResponse
        {
            Token = token,
            UserId = result.Data.Id,
            UserName = result.Data.UserName
        };

        return TypedResults.Ok(response);
    }

    internal class Validator : AbstractValidator<Parameters>
    {
        public Validator()
        {
            this.RuleFor(x => x.Request.Login)
                .NotEmpty();

            this.RuleFor(x => x.Request.Password)
                .NotEmpty();
        }
    }

    internal readonly struct Parameters
    {
        [FromBody]
        public required LoginRequest Request { get; init; }
    }

    internal readonly struct Services
    {
        [FromServices]
        public required IUserRepository UserRepository { get; init; }

        [FromServices]
        public required ITokenService TokenService { get; init; }

        public required CancellationToken CancellationToken { get; init; }
    }
}
