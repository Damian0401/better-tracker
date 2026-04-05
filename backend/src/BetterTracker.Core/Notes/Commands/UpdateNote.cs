using BetterTracker.Contracts;
using BetterTracker.Data.Repositories;

namespace BetterTracker.Core.Notes.Commands;

public static class UpdateNote
{
    public static async Task<bool> HandleAsync(
        UpdateNoteRequest request,
        Guid userId,
        INoteRepository noteRepository,
        CancellationToken cancellationToken)
    {
        var note = await noteRepository.GetByIdAsync(request.Id, cancellationToken);
        if (note is null || note.UserId != userId)
        {
            return false;
        }

        note.Title = request.Title;
        note.Content = request.Content;

        noteRepository.Update(note);
        await noteRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
