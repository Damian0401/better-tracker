using Microsoft.EntityFrameworkCore;

namespace BetterTracker.Data;

public sealed class NoteRepository : INoteRepository
{
    private readonly AppDbContext dbContext;

    public NoteRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<NoteEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await this.dbContext.Notes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public void Add(NoteEntity note)
    {
        this.dbContext.Notes.Add(note);
    }

    public void Update(NoteEntity note)
    {
        this.dbContext.Notes.Update(note);
    }

    public void Remove(NoteEntity note)
    {
        this.dbContext.Notes.Remove(note);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await this.dbContext.SaveChangesAsync(cancellationToken);
    }
}
