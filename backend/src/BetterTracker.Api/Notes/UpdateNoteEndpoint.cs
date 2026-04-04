using Asp.Versioning;
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
        builder.MapPut("/notes/{id:guid}", HandleAsync).WithValidation<Parameters>();

    private static async ValueTask<NoContent> HandleAsync(
        [AsParameters] Parameters parameters,
        [AsParameters] Services services)
    {
        var request = new UpdateNoteRequest
        {
            Id = parameters.Id,
            Title = parameters.Request.Title,
            Content = parameters.Request.Content,
        };

        await UpdateNote.HandleAsync(
            request,
            services.NoteRepository,
            services.CancellationToken);

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

        public required CancellationToken CancellationToken { get; init; }
    }
}
