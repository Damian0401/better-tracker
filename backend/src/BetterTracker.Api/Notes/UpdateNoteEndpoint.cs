using System.Security.Claims;
using Asp.Versioning;
using BetterTracker.Common.Helpers;
using BetterTracker.Contracts;
using BetterTracker.Core.Notes.Commands;
using BetterTracker.Data.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BetterTracker.Api.Notes;

public class UpdateNoteEndpoint : IApiEndpoint
{
    public ApiVersion Version => ApiVersions.V1;
    public string DefaultTag => ApiTags.Notes;

    public IEndpointConventionBuilder Register(IEndpointRouteBuilder builder) =>
        builder.MapPut("/notes/{id:guid}", HandleAsync).WithValidation<Parameters>().RequireAuthorization();

    private static async ValueTask<Results<NoContent, NotFound, UnauthorizedHttpResult>> HandleAsync(
        [AsParameters] Parameters parameters,
        [AsParameters] Services services)
    {
        var userIdResult = UserIdHelper.GetUserId(services.HttpContextAccessor.HttpContext!);
        if (!userIdResult.IsSuccess)
        {
            return TypedResults.Unauthorized();
        }

        var request = new UpdateNoteRequest
        {
            Id = parameters.Id,
            Title = parameters.Request.Title,
            Content = parameters.Request.Content,
        };

        var noteFound = await UpdateNote.HandleAsync(
            request,
            userIdResult.Data,
            services.NoteRepository,
            services.CancellationToken);

        if (!noteFound)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.NoContent();
    }

    internal class Validator : AbstractValidator<Parameters>
    {
        public Validator()
        {
            this.RuleFor(x => x.Request.Title)
                .NotEmpty()
                .MaximumLength(100);

            this.RuleFor(x => x.Request.Content)
                .NotEmpty()
                .MaximumLength(500);
        }
    }

    internal readonly struct Parameters
    {
        [FromRoute]
        public required Guid Id { get; init; }

        [FromBody]
        public required UpdateNoteBody Request { get; init; }
    }

    internal sealed record UpdateNoteBody
    {
        public required string Title { get; init; }
        public required string Content { get; init; }
    }

    internal readonly struct Services
    {
        [FromServices]
        public required INoteRepository NoteRepository { get; init; }

        [FromServices]
        public required IHttpContextAccessor HttpContextAccessor { get; init; }

        public required CancellationToken CancellationToken { get; init; }
    }
}
