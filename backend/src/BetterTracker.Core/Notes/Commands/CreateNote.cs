using BetterTracker.Common.Results;
using BetterTracker.Contracts;
using BetterTracker.Data.Entities;
using BetterTracker.Data.Repositories;

namespace BetterTracker.Core.Notes.Commands;

public static class CreateNote
{
    public static async Task<Result<CreateNoteResponse>> HandleAsync(
        CreateNoteRequest request,
        Guid userId,
        INoteRepository noteRepository,
        CancellationToken cancellationToken)
    {
        var note = new NoteEntity
        {
            Title = request.Title,
            Content = request.Content,
            UserId = userId,
        };

        noteRepository.Add(note);
        await noteRepository.SaveChangesAsync(cancellationToken);

        return Result<CreateNoteResponse>.Success(new CreateNoteResponse { Id = note.Id });
    }
}
