using BetterTracker.Contracts;
using BetterTracker.Data.Repositories;

namespace BetterTracker.Core.Notes.Commands;

public static class DeleteNote
{
    public static async Task HandleAsync(
        DeleteNoteRequest request,
        INoteRepository noteRepository,
        CancellationToken cancellationToken)
    {
        var note = await noteRepository.GetByIdAsync(request.Id, cancellationToken);
        if (note is null)
        {
            return;
        }

        noteRepository.Remove(note);
        await noteRepository.SaveChangesAsync(cancellationToken);
    }
}
