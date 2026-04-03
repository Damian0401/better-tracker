using Asp.Versioning;
using BetterTracker.Contracts;
using BetterTracker.Core.Notes.Commands;
using BetterTracker.Data;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BetterTracker.Api.Notes;

public class CreateNoteEndpoint : IApiEndpoint
{
    public ApiVersion Version => ApiVersions.V1;
    public string DefaultTag => ApiTags.Notes;

    public IEndpointConventionBuilder Register(IEndpointRouteBuilder builder) =>
        builder.MapPost("/notes", HandleAsync).WithValidation<Parameters>();

    private static async ValueTask<NoContent> HandleAsync(
        [AsParameters] Parameters parameters,
        [AsParameters] Services services)
    {
        await CreateNote.HandleAsync(
            parameters.Request,
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
        [FromBody]
        public required CreateNoteRequest Request { get; init; }
    }

    internal readonly struct Services
    {
        [FromServices]
        public required INoteRepository NoteRepository { get; init; }

        public required CancellationToken CancellationToken { get; init; }
    }
}
