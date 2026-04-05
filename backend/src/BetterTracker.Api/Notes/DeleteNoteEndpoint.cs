using System.Security.Claims;
using Asp.Versioning;
using BetterTracker.Common.Helpers;
using BetterTracker.Contracts;
using BetterTracker.Core.Notes.Commands;
using BetterTracker.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BetterTracker.Api.Notes;

public class DeleteNoteEndpoint : IApiEndpoint
{
    public ApiVersion Version => ApiVersions.V1;
    public string DefaultTag => ApiTags.Notes;

    public IEndpointConventionBuilder Register(IEndpointRouteBuilder builder) =>
        builder.MapDelete("/notes/{id:guid}", HandleAsync).RequireAuthorization();

    private static async ValueTask<Results<NoContent, NotFound, UnauthorizedHttpResult>> HandleAsync(
        [AsParameters] Parameters parameters,
        [AsParameters] Services services)
    {
        var userIdResult = UserIdHelper.GetUserId(services.HttpContextAccessor.HttpContext!);
        if (!userIdResult.IsSuccess)
        {
            return TypedResults.Unauthorized();
        }

        var request = new DeleteNoteRequest
        {
            Id = parameters.Id,
        };

        var noteFound = await DeleteNote.HandleAsync(
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

    internal readonly struct Parameters
    {
        [FromRoute]
        public required Guid Id { get; init; }
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
