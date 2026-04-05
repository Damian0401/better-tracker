using BetterTracker.Contracts;
using BetterTracker.Data.Repositories;

namespace BetterTracker.Core.Notes.Commands;

public static class DeleteNote
{
    public static async Task<bool> HandleAsync(
        DeleteNoteRequest request,
        Guid userId,
        INoteRepository noteRepository,
        CancellationToken cancellationToken)
    {
        var note = await noteRepository.GetByIdAsync(request.Id, cancellationToken);
        if (note is null || note.UserId != userId)
        {
            return false;
        }

        noteRepository.Remove(note);
        await noteRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
