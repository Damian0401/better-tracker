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
        builder.MapDelete("/notes/{id:guid}", HandleAsync);

    private static async ValueTask<NoContent> HandleAsync(
        [AsParameters] Parameters parameters,
        [AsParameters] Services services)
    {
        var request = new DeleteNoteRequest
        {
            Id = parameters.Id,
        };

        await DeleteNote.HandleAsync(
            request,
            services.NoteRepository,
            services.CancellationToken);

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

        public required CancellationToken CancellationToken { get; init; }
    }
}
