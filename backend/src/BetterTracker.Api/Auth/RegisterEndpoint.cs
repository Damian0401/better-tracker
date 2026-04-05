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

public class RegisterEndpoint : IApiEndpoint
{
    public ApiVersion Version => ApiVersions.V1;
    public string DefaultTag => ApiTags.Auth;

    public IEndpointConventionBuilder Register(IEndpointRouteBuilder builder) =>
        builder.MapPost("/auth/register", HandleAsync).WithValidation<Parameters>();

    private static async ValueTask<Ok<AuthResponse>> HandleAsync(
        [AsParameters] Parameters parameters,
        [AsParameters] Services services)
    {
        var user = await RegisterUser.HandleAsync(
            parameters.Request,
            services.UserRepository,
            services.CancellationToken);

        var token = services.TokenService.GenerateToken(user.Id, user.UserName);

        var response = new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            UserName = user.UserName
        };

        return TypedResults.Ok(response);
    }

    internal class Validator : AbstractValidator<Parameters>
    {
        public Validator()
        {
            this.RuleFor(x => x.Request.UserName)
                .NotEmpty()
                .MaximumLength(100);

            this.RuleFor(x => x.Request.Login)
                .NotEmpty()
                .MaximumLength(100);

            this.RuleFor(x => x.Request.Password)
                .NotEmpty()
                .MinimumLength(6);
        }
    }

    internal readonly struct Parameters
    {
        [FromBody]
        public required RegisterRequest Request { get; init; }
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
