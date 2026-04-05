using System.Security.Claims;
using Asp.Versioning;
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

    private static async ValueTask<Results<NoContent, NotFound>> HandleAsync(
        [AsParameters] Parameters parameters,
        [AsParameters] Services services)
    {
        var userIdClaim = services.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }

        var request = new DeleteNoteRequest
        {
            Id = parameters.Id,
        };

        var noteFound = await DeleteNote.HandleAsync(
            request,
            userId,
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
        public required HttpContext HttpContext { get; init; }

        public required CancellationToken CancellationToken { get; init; }
    }
}
