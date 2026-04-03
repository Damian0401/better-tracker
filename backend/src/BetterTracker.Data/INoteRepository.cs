namespace BetterTracker.Data;

public interface INoteRepository
{
    Task<NoteEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(NoteEntity note);
    void Update(NoteEntity note);
    void Remove(NoteEntity note);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
